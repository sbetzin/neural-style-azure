#!/usr/bin/env python
import time
import json
import os
import sys
import os.path
import logging
import argparse
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

logger, file_handler = create_logger("mask_transfer")

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
        
        video_name = job["video_name"]
        
        logger.info(f"start mask-transfer in path {video_name}")
        command_line = create_commandline_from_job("/app/main.py", job)
        run_python(command_line)

    except Exception as e:
        logger.exception(e)
 
def read_stdout(process):
    for line in process.stdout:
        logger.info(line.strip())

def create_commandline_from_job(command, job):
    command_line = ["python", command]

    for key, value in job.items():
        if value == False:
            continue

        if value is True:
            command_line.append(f"--{key}")
        else:
            command_line.append(f"--{key}={value}")

    
    return command_line

def run_python(command_line):
    logger.info(f'Starting command: {" ".join(command_line)}')

    process = subprocess.Popen(command_line, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, bufsize=0)

    stdout_thread = threading.Thread(target=read_stdout, args=(process,))
    stdout_thread.start()

    _, stderr = process.communicate()
    stdout_thread.join()

    if stderr:
        logger.error(f"Fehlermeldungen:\n{stderr}")
 


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
        
    queue_name ="jobs-mask-transfer"
    queue_client = setup_azure_queue(azure_connection_string, queue_name)

    logger.info ("preparing azure resources")
    prepare_queue(queue_client, queue_name)

    logger.info ("starting to poll jobs queue: %s", queue_name)
    poll_queue(queue_client)

if __name__ == '__main__':
  main(sys.argv[1:])