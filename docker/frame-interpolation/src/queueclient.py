#!/usr/bin/env python
import time
import json
import os
import glob
import sys
import os.path
import logging
import argparse
import yaml

from azure.storage.queue import QueueClient
from azure.storage.blob import BlobServiceClient, BlobClient
from azure.core.exceptions import ResourceExistsError

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("queueclient")
logger.setLevel(logging.INFO)

# Only show error messages for the azure storage. otherwise the console is spammed with debug messages
azure_logger = logging.getLogger("azure.storage")
azure_logger.setLevel(logging.ERROR)

# Only show error messages for the azure core. otherwise the console is spammed with debug messages
azure_logger = logging.getLogger("azure.core")
azure_logger.setLevel(logging.ERROR)

#insights.enable_logging()
#telemetrie = insights.create_telemetrie_client()

def ensure_dir(file_path):
    directory = os.path.dirname(file_path)
    if not os.path.exists(directory):
        os.makedirs(directory)

def prepare_queue(queue_client, queue_name):
    try:
        queue_client.create_queue()
        logger.info("creating queue: %s", queue_name)
    except ResourceExistsError:
        logger.info("Queue already exists.")
    except Exception as e:
        logger.exception(e)

def prepare_blob(blob_service_client, container_name):
    try:
        blob_service_client.create_container(container_name)
    except ResourceExistsError:
        logger.info("Container already exists.")
    except Exception as e:
        logger.exception(e)

def handle_message(blob_service_client, message):
    try:
        logger.info("--------------------------------------------------------------------------------------------------")
        logger.info("handling new message %s", message.id )
        
        job = json.loads(message.content)
        #telemetrie.track_event ("new image", job)
        
        content_name = job["content_name"]
        recreate_depth_mesh = job["recreate_depth_mesh"]
        depth_mode = job["depth_mode"]
        
        directory_content = "image"
        directory_result = "video"
        directory_mesh = f"/mesh/{depth_mode}"
        directory_depth = f"/depth/{depth_mode}"
        
        job["mesh_folder"] = directory_mesh
        job["depth_folder"] = directory_depth
         
        os.makedirs(directory_content, exist_ok=True)
        os.makedirs(directory_result, exist_ok=True)
        
        content_file = os.path.join(directory_content, content_name)
        
        # Delete all existing images. Otherwise the 3d-inpainting would iterate them all
        clear_directory(directory_content)
        handle_mesh_deletion(directory_depth, directory_mesh, content_name, recreate_depth_mesh)
        update_yaml_file('default.yml', job)
        
        logger.info("downloading %s", content_file)
        download_file(blob_service_client, content_name, content_file)

        logger.info("start 3d-inpainting with Content=%s", content_name)
        command = f'python main.py --config default.yml'
        os.system(command)
 
        logger.info("Setting exif data")
        #exifdump.write_exif(out_file_origcolor_0, config)
        
        upload_videos(blob_service_client, directory_result)    
        upload_depth(blob_service_client, directory_depth, replace_file_extension(content_name, '.png'), depth_mode)    
    except Exception as e:
        logger.exception(e)

def handle_mesh_deletion(directory_depth, directory_mesh, content_name, recreate_depth_mesh):
    if not recreate_depth_mesh:
        return
    
    # Split the filename into name and extension
    mesh_file_name = replace_file_extension(content_name, '.ply')
    mesh_file = os.path.join(directory_mesh, mesh_file_name)
    
    if os.path.exists(mesh_file):
        print(f"removing mesh file: {mesh_file_name}")
        os.remove(mesh_file)
        
    depth_file_name = replace_file_extension(content_name, '.npy')
    depth_file = os.path.join(directory_depth, depth_file_name)
    
    if os.path.exists(depth_file):
        print(f"removing depth file: {mesh_file_name}")
        os.remove(depth_file)
        
    
def replace_file_extension(target_file, new_extension):
    # Split the filename into name and extension
    file_root, _ = os.path.splitext(target_file)

    # Join the root with the new extension
    new_filename = file_root + new_extension
    
    return new_filename
 
def clear_directory(target_path):
    files = glob.glob(os.path.join(target_path, "*.jpg"))

    for file in files:
        os.remove(file)

def upload_videos(blob_service_client, directory_result):
    files = glob.glob(os.path.join(directory_result, "*.mp4"))
    
    for file in files:
        target_name = os.path.basename(file)
        print(f"Uploading {file}")
        upload_file(blob_service_client,"results", target_name, file)
        os.remove(file)

def upload_depth(blob_service_client, directory_depth, depth_file_name, depth_mode):
    print(f"Uploading {depth_file_name}")
    depth_file = os.path.join(directory_depth, depth_file_name)
    container_file = f"{depth_mode}/{depth_file_name}"
    
    upload_file(blob_service_client, "depth", container_file, depth_file)
     
def download_file(blob_service_client, source_name, source_file):
    blob_client = blob_service_client.get_blob_client(container="images", blob=source_name)
    
    with open(source_file, "wb") as download_file:
        download_file.write(blob_client.download_blob().readall())

def upload_file(blob_service_client, container, target_name, file_name):
    try:
        if os.path.exists(file_name):
            logger.info ("uploading file %s", file_name)
            
            blob_client = blob_service_client.get_blob_client(container=container, blob=target_name)
            with open(file_name, "rb") as data:
                blob_client.upload_blob(data, overwrite=True)

        else:
            logger.info("file %s does not exit", file_name)
    except Exception as e:
        logger.exception(e)

def poll_queue(queue_client, blob_service_client):
    try:
        while True:
            CheckQueue(queue_client, blob_service_client)
                
            time.sleep(5)
    except Exception as e:
        logger.exception(e)

def CheckQueue(queue_client, blob_service_client):
    messages = queue_client.receive_messages(messages_per_page=1, visibility_timeout=30*60)

    for message_batch in messages.by_page():
        for message in message_batch:
            start_time = time.time()

            handle_message(blob_service_client, message)
            measure_time(start_time)

            queue_client.delete_message(message)
            
            return True
    return False

def setup_azure_queue(azure_connection_string, queue_name):
    try:
        queue_client = QueueClient.from_connection_string(conn_str=azure_connection_string, queue_name=queue_name)
    except Exception as e:
        logger.exception(e)

    return queue_client

def setup_azure_blob(azure_connection_string):
    try:
        blob_service_client = BlobServiceClient.from_connection_string(azure_connection_string)
    except Exception as e:
        logger.exception(e)

    return blob_service_client

def update_yaml_file(yaml_file, new_values):
    # YAML-Datei einlesen
    with open(yaml_file, 'r') as file:
        config = yaml.safe_load(file)

    # Dictionary-Elemente aktualisieren
    for key, value in new_values.items():
        config[key] = value

    # YAML-Datei speichern
    with open(yaml_file, 'w') as file:
        yaml.safe_dump(config, file)
        
def measure_time(start_time):
    generation_time = time.time() - start_time

    #telemetrie.track_metric('generation time', generation_time)
    logger.info("took %s seconds" % generation_time)

def parse_args(argv):

    desc = "QueueClient for tensorflow implementation of Neural-Style"  
    parser = argparse.ArgumentParser(description=desc)

    parser.add_argument('--queue_name', type=str, default='jobs-3d-inpainting', help='name of the queue to poll')
    args = parser.parse_args(argv)

    return args

def main(argv):
    logger.info("parsing arguments")
    args = parse_args(argv)
    
    azure_connection_string = os.getenv("AzureStorageConnectionString")
    if azure_connection_string == None:
        raise NameError("environment variable AzureStorageConnectionString is not set")

    if not os.getenv("AzureStorageQueueName") == None:
        args.queue_name = os.getenv("AzureStorageQueueName")
        
    queue_name ="jobs-3d-inpainting"

    queue_client = setup_azure_queue(azure_connection_string, queue_name)
    blob_service_client = setup_azure_blob(azure_connection_string)
  
    logger.info ("preparing azure resources")
    prepare_queue(queue_client, queue_name)
    prepare_blob(blob_service_client, "images")
    prepare_blob(blob_service_client, "results")

    logger.info ("starting to poll jobs queue: %s", queue_name)
    poll_queue(queue_client, blob_service_client)

if __name__ == '__main__':
  main(sys.argv[1:])