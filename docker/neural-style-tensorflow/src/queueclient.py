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
from neural_style import main as neural_style_calc
from azure.core.exceptions import ResourceExistsError

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("queueclient")
logger.setLevel(logging.INFO)

# Only show error messages for the azure storage. otherwise the console is spammed with debug messages
azure_logger = logging.getLogger("azure.storage")
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
        logger.error(e)

def prepare_blob(blob_service_client, container_name):
    try:
        blob_service_client.create_container(container_name)
    except ResourceExistsError:
        logger.info("Container already exists.")
    except Exception as e:
        logger.error(e)

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
        size = job["Size"]
        iterations = job["Iterations"]
        model = job["Model"]

        telemetrie.track_event ("new image", job)

        image_dir = "/app/images/"
        source_file = os.path.join(image_dir, source_name)
        style_file =  os.path.join(image_dir, style_name)

        args = ["--content_img", source_file]
        args.extend(["--style_imgs", style_file])
        args.extend(["--img_name", target_name])
        args.extend(["--content_weight", str(content_weight)])
        args.extend(["--style_weight", str(style_weight)])
        args.extend(["--max_size", str(size)])
        args.extend(["--max_iterations", str(iterations)])
        args.extend(["--img_output_dir", image_dir])
        args.extend(["--model_weights", model])
        args.extend(["--verbose"])

        logger.info("downloading %s", source_file )
        blob_client = blob_service_client.get_blob_client(container="images", blob=source_name)
        with open(source_file, "wb") as download_file:
            download_file.write(blob_client.download_blob().readall())

        logger.info("downloading %s", style_file )
        blob_client = blob_service_client.get_blob_client(container="images", blob=style_name)
        with open(style_file, "wb") as download_file:
            download_file.write(blob_client.download_blob().readall())


        logger.info("start job with Source=%s, Style=%s, Target=%s, Size=%s, Model=%s", source_name, style_name, target_name, size, model)

        target_name_origcolor_0 = target_name.replace("#origcolor#", "0")
        target_name_origcolor_1 = target_name.replace("#origcolor#", "1")

        out_file_origcolor_0 =  os.path.join(image_dir, target_name_origcolor_0)
        out_file_origcolor_1 =  os.path.join(image_dir, target_name_origcolor_1)

        neural_style_calc(args)

        upload_file(blob_service_client, target_name_origcolor_0, out_file_origcolor_0)
        upload_file(blob_service_client, target_name_origcolor_1, out_file_origcolor_1)
    except Exception as e:
        logger.error(e)

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
        logger.error(e)

def poll_queue(queue_client, blob_service_client, queue_name):
    try:
        logger.info ("starting to poll jobs queue: %s", queue_name)
        while True:
            messages = queue_client.receive_messages(messages_per_page=1, visibility_timeout=30*60)

            for message_batch in messages.by_page():
                for message in message_batch:
                    start_time = time.time()

                    handle_message(blob_service_client, message)
                    measure_time(start_time)

                    queue_client.delete_message(message)
                
            time.sleep(5)
    except Exception as e:
        logger.error(e)

def setup_azure(azure_connection_string, queue_name):
    try:
        queue_client = QueueClient.from_connection_string(conn_str=azure_connection_string, queue_name=queue_name)
        blob_service_client = BlobServiceClient.from_connection_string(azure_connection_string)
    except Exception as e:
        logger.error(e)

    return (queue_client, blob_service_client)

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

    queue_client, blob_service_client = setup_azure(azure_connection_string, args.queue_name)

    logger.info ("preparing azure resources")
    prepare_queue(queue_client, args.queue_name)
    prepare_blob(blob_service_client, "images")
    prepare_blob(blob_service_client, "results")

    logger.info ("ensuring directory exists")
    ensure_dir("/app/images/")

    logger.info ("starting queue client")
    poll_queue(queue_client, blob_service_client, args.queue_name)

if __name__ == '__main__':
  main(sys.argv[1:])