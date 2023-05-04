#!/usr/bin/env python
import time
import json
import os
import glob
import sys
import os.path
import logging
import argparse
import subprocess

from azure.storage.queue import QueueClient
from azure.core.exceptions import ResourceExistsError

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("queueclient")
logger.setLevel(logging.INFO)

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

def handle_message(message):
    try:
        logger.info("--------------------------------------------------------------------------------------------------")
        logger.info("handling new message %s", message.id )
        job = json.loads(message.content)
        #telemetrie.track_event ("new image", job)
        
        target_path = job["target_path"]
        
        logger.info(f"start frame-interpolation in path {target_path}")
        command = "python /app/main.py"

        for key, value in job.items():
            command += f" --{key}={value}"

        logger.info(f'Starting command={command}')

        process = subprocess.Popen(command, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
        stdout, stderr = process.communicate()

        logger.info("Output des Skripts:\n", stdout.decode("utf-8"))
        if stderr:
            logger.error("Fehlermeldungen:\n", stderr.decode("utf-8"))
 
        logger.info("Setting exif data")
        #exifdump.write_exif(out_file_origcolor_0, config)
    except Exception as e:
        logger.exception(e)
 

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

        
def measure_time(start_time):
    generation_time = time.time() - start_time

    #telemetrie.track_metric('generation time', generation_time)
    logger.info("took %s seconds" % generation_time)

def parse_args(argv):
    desc = "QueueClient for frame-interpolation"  
    parser = argparse.ArgumentParser(description=desc)

    parser.add_argument('--queue_name', type=str, default='jobs-frame-interpolation', help='name of the queue to poll')
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
        
    queue_name ="jobs-frame-interpolation"

    queue_client = setup_azure_queue(azure_connection_string, queue_name)

    logger.info ("preparing azure resources")
    prepare_queue(queue_client, queue_name)

    logger.info ("starting to poll jobs queue: %s", queue_name)
    poll_queue(queue_client)

if __name__ == '__main__':
  main(sys.argv[1:])