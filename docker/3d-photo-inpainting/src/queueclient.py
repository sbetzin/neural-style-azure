#!/usr/bin/env python
import time
import json
import os
import sys
import os.path
import logging
import argparse
import insights
import exifdump
import image_tools

from azure.storage.queue import QueueClient
from azure.storage.blob import BlobServiceClient, BlobClient
from neural_style_transfer import neural_style_transfer as neural_style_transfer
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

insights.enable_logging()
telemetrie = insights.create_telemetrie_client()

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
        telemetrie.track_event ("new image", job)
        
        content_name = job["ContentName"]
        style_name = job["StyleName"]
        target_name_origcolor_0 = job["TargetName"].replace("#origcolor#", "0")
        target_name_origcolor_1 = job["TargetName"].replace("#origcolor#", "1")
               
        directory_content = "/app/images/in/"
        directory_style = "/app/images/style/"
        directory_out = "/app/images/out/"
        os.makedirs(directory_content, exist_ok=True)
        os.makedirs(directory_style, exist_ok=True)
        os.makedirs(directory_out, exist_ok=True)
        
        content_file = os.path.join(directory_content, content_name)
        style_file =  os.path.join(directory_style, style_name)

        out_file_origcolor_0 = os.path.join(directory_out, target_name_origcolor_0)
        out_file_origcolor_1 = os.path.join(directory_out, target_name_origcolor_1)
        
        config = create_config(directory_content, directory_style, directory_out, content_name, style_name, out_file_origcolor_0)
        transfer_job_param_to_config(job, config)
        
        logger.info("downloading %s and %s", content_file, style_file )
        download_file(blob_service_client, content_name, content_file)
        download_file(blob_service_client, style_name, style_file)

        logger.info("calculating target shape with max_size=%s", job["Size"])
        target_shape = image_tools.find_target_size(content_file, job["Size"])
        config['target_shape'] = target_shape
        
        logger.info("start style transfer with Source=%s, Style=%s, Target=%s", content_name, style_name, out_file_origcolor_0)
        neural_style_transfer(logger, config)

        logger.info("creating original colors")
        image_tools.create_image_with_original_colors(content_file, out_file_origcolor_0, out_file_origcolor_1)
        
        logger.info("Setting exif data")
        exifdump.write_exif(out_file_origcolor_0, config)
        exifdump.write_exif(out_file_origcolor_1, config)
        
        upload_file(blob_service_client, target_name_origcolor_0, out_file_origcolor_0)
        upload_file(blob_service_client, target_name_origcolor_1, out_file_origcolor_1)
    except Exception as e:
        logger.exception(e)

def create_config(directory_content, directory_style, directory_out, content_name, style_name, out_file_origcolor_0):
    config = dict()
    config['content_images_dir'] = directory_content
    config['style_images_dir'] = directory_style
    config['output_img_dir'] = directory_out
    config['content_img_name'] = content_name
    config['style_img_name'] = style_name
    config['output_img_name'] = out_file_origcolor_0
    config['saving_freq'] = -1
    return config

def transfer_job_param_to_config(job, config):
    config["iterations"] = job["Iterations"]
    config['img_format'] = (4, '.jpg') # saves images in the format: %04d.jpg
    config['content_weight'] = job["ContentWeight"] 
    config['style_weight'] = job["StyleWeight"]
    config['tv_weight'] = job["TvWeight"]
    config['optimizer'] = job["Optimizer"]
    config['model'] =  job["Model"]
    config['init_method'] = job["Init"]
    config['max_size'] = job["Size"]

def download_file(blob_service_client, source_name, source_file):
    blob_client = blob_service_client.get_blob_client(container="images", blob=source_name)
    
    with open(source_file, "wb") as download_file:
        download_file.write(blob_client.download_blob().readall())

def upload_file(blob_service_client, target_name, file_name):
    try:
        if os.path.exists(file_name):
            logger.info ("uploading file %s", file_name)
            
            blob_client = blob_service_client.get_blob_client(container="results", blob=target_name)
            with open(file_name, "rb") as data:
                blob_client.upload_blob(data, overwrite=True)

        else:
            logger.info("file %s does not exit", file_name)
    except Exception as e:
        logger.exception(e)

def poll_queue(queue_client, priorityQueue_client, blob_service_client):
    try:
        while True:
           
            hadPriorityMessages = CheckQueue(priorityQueue_client, blob_service_client)
            if not hadPriorityMessages:
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

def measure_time(start_time):
    generation_time = time.time() - start_time

    telemetrie.track_metric('generation time', generation_time)
    logger.info("took %s seconds" % generation_time)

def parse_args(argv):

    desc = "QueueClient for tensorflow implementation of Neural-Style"  
    parser = argparse.ArgumentParser(description=desc)

    parser.add_argument('--queue_name', type=str, default='jobs', help='name of the queue to poll')
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
        
    queue_name ="test"
    priority_queue_name = "priority-jobs"

    queue_client = setup_azure_queue(azure_connection_string, queue_name)
    priority_queue_client = setup_azure_queue(azure_connection_string, priority_queue_name)
    blob_service_client = setup_azure_blob(azure_connection_string)
  
    logger.info ("preparing azure resources")
    prepare_queue(queue_client, queue_name)
    prepare_queue(priority_queue_client, priority_queue_name)
    prepare_blob(blob_service_client, "images")
    prepare_blob(blob_service_client, "results")

    logger.info ("ensuring directory exists")
    ensure_dir("/app/images/")

    logger.info ("starting to poll jobs queue: %s", queue_name)
    poll_queue(queue_client, priority_queue_client, blob_service_client)

if __name__ == '__main__':
  main(sys.argv[1:])