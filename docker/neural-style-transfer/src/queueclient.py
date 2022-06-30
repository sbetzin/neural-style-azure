#!/usr/bin/env python
import datetime
import time
import json
import base64
import os
import sys
import os.path
import logging
import numpy
import argparse
import insights

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
        source_name = job["SourceName"]
        style_name = job["StyleName"]
        target_name = job["TargetName"]
        style_weight = job["StyleWeight"]
        content_weight = job["ContentWeight"]
        tv_weight = job["TvWeight"]
        temporal_weight = job["TemporalWeight"]
        content_loss_function = job["ContentLossFunction"]
        size = job["Size"]
        iterations = job["Iterations"]
        model = job["Model"]

        telemetrie.track_event ("new image", job)

        image_dir_in = "/app/images/in/"
        image_dir_style = "/app/images/style/"
        image_dir_out = "/app/images/out/"
        img_format = (4, '.jpg')  # saves images in the format: %04d.jpg
         
        source_file = os.path.join(image_dir_in, source_name)
        style_file =  os.path.join(image_dir_style, style_name)

        config = dict()
        config['content_images_dir'] = image_dir_in
        config['style_images_dir'] = image_dir_style
        config['output_img_dir'] = image_dir_out
        config['img_format'] = img_format


        config['content_img_name'] = source_file
        config['style_img_name'] = style_file
        config['height'] = str(size)
        config['content_weight'] = str(content_weight)
        config['style_weight'] = str(style_weight)
        config['tv_weight'] = str(tv_weight)
        config['optimizer'] = "lbfgs"
        config['model'] = "vgg19"
        config['init_method'] = "content"
        config['saving_freq'] = "-1"
    
        # args.extend(["--img_name", target_name])
        # args.extend(["--device","/gpu:0"])

        logger.info("downloading %s", source_file )
        download_file(blob_service_client, source_name, source_file)

        logger.info("downloading %s", style_file )
        download_file(blob_service_client, style_name, style_file)


        logger.info("start job with Source=%s, Style=%s, Target=%s, Size=%s, Model=%s", source_name, style_name, target_name, size, model)

        target_name_origcolor_0 = target_name.replace("#origcolor#", "0")
        target_name_origcolor_1 = target_name.replace("#origcolor#", "1")

        out_file_origcolor_0 =  os.path.join(image_dir_out, target_name_origcolor_0)
        out_file_origcolor_1 =  os.path.join(image_dir_out, target_name_origcolor_1)

        neural_style_transfer(config)

        upload_file(blob_service_client, target_name_origcolor_0, out_file_origcolor_0)
        upload_file(blob_service_client, target_name_origcolor_1, out_file_origcolor_1)
    except Exception as e:
        logger.exception(e)

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
                blob_client.upload_blob(data)

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

    queue_client, priority_queue_client, blob_service_client = setup_azure_queue(azure_connection_string, queue_name)
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