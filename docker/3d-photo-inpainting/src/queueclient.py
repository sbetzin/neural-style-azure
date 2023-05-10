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
import shutil
import subprocess
import threading
from azure.storage.queue import QueueClient
from azure.core.exceptions import ResourceExistsError

def create_logger(name):
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
        datefmt="%Y-%m-%d %H:%M:%S"
    )

    logger = logging.getLogger(name)
    logger.setLevel(logging.INFO)

    file_handler = logging.FileHandler(f"/nft/log/{name}.log")
    formatter = logging.Formatter("%(asctime)s - %(name)s - %(levelname)s - %(message)s", datefmt="%Y-%m-%d %H:%M:%S")
    file_handler.setFormatter(formatter)

    logger.addHandler(file_handler)

    # Only show error messages for the azure core. otherwise the console is spammed with debug messages
    azure_logger = logging.getLogger("azure.core")
    azure_logger.setLevel(logging.ERROR)

    return logger, file_handler

logger, file_handler = create_logger("3d_inpainting")

#insights.enable_logging()
#telemetrie = insights.create_telemetrie_client()

def prepare_queue(queue_client, queue_name):
    try:
        queue_client.create_queue()
        logger.info("creating queue: %s", queue_name)
    except ResourceExistsError:
        logger.info("Queue already exists.")
    except Exception as e:
        logger.exception(e)

def handle_message(message):
    try:
        logger.info("--------------------------------------------------------------------------------------------------")
        logger.info("handling new message %s", message.id )
        
        job = json.loads(message.content)
        #telemetrie.track_event ("new image", job)
        
        content_name = job["content_name"]
        recreate_depth_mesh = job["recreate_depth_mesh"]
        depth_mode = job["depth_mode"]
        
        image_folder = "/nft/in"
        local_image_folder = "/image"
        video_folder = "/nft/video/_in"
        mesh_folder = f"/mesh/{depth_mode}"
        depth_folder = f"/depth/{depth_mode}"
        
        job["src_folder"] = local_image_folder
        job["mesh_folder"] = mesh_folder
        job["depth_folder"] = depth_folder
        job["video_folder"] = video_folder
        
        os.makedirs(local_image_folder, exist_ok=True)
        os.makedirs(video_folder, exist_ok=True)
        
        content_file = find_image_file(image_folder, content_name)
        local_content_file = os.path.join(local_image_folder, content_name)
        
        # Delete all local existing images. Otherwise the 3d-inpainting would iterate them all
        clear_directory(local_image_folder)
        handle_mesh_deletion(depth_folder, mesh_folder, content_name, recreate_depth_mesh)
        update_yaml_file('default.yml', job)
        
        logger.info(f"copying {content_file} to {local_content_file}")
        shutil.copyfile(content_file, local_content_file)

        logger.info(f"start 3d-inpainting with Content={content_name}")
        command_line = ["python", "/app/main.py"]
        command_line.append(f"--config=default.yml")

        run_python(command_line)
 
        logger.info("Setting exif data")
        #exifdump.write_exif(out_file_origcolor_0, config)
    except Exception as e:
        logger.exception(e)


def run_python(command_line):
    logger.info(f'Starting command: {" ".join(command_line)}')

    process = subprocess.Popen(command_line, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)

    while process.poll() is None:  # solange der Prozess läuft
        output = process.stdout.readline().strip()
        if output:
            logger.info(output)

    _, stderr = process.communicate()

    if stderr:
        logger.error(f"Fehlermeldungen:\n{stderr}")

 
        
def find_image_file(folder_path, content_name):
    for root, _, files in os.walk(folder_path):
        for file in files:
            if file.lower().endswith(".jpg") and file == content_name:
                return os.path.join(root, file)

    return None

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

def poll_queue(queue_client):
    try:
        while True:
            CheckQueue(queue_client)
                
            time.sleep(5)
    except Exception as e:
        logger.exception(e)

def CheckQueue(queue_client):
    messages = queue_client.receive_messages(messages_per_page=1, visibility_timeout=30*60)

    for message_batch in messages.by_page():
        for message in message_batch:
            start_time = time.time()

            handle_message(message)
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
    desc = "QueueClient for tensorflow implementation 3d-photo-inpainting"  
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
  
    logger.info ("preparing azure resources")
    prepare_queue(queue_client, queue_name)

    logger.info ("starting to poll jobs queue: %s", queue_name)
    poll_queue(queue_client)

if __name__ == '__main__':
  main(sys.argv[1:])