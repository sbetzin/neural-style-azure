#!/usr/bin/env python
import datetime
import time
import json
import base64
import os
import os.path
import logging
import numpy

from azure.storage.queue import QueueService
from azure.storage.blob import BlockBlobService
from neural_style import main as neural_style_calc

env_connection = os.environ['AzureStorageConnectionString']

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("queueclient")

azure_logger = logging.getLogger('azure.storage')
azure_logger.setLevel(logging.ERROR)

try:
    queue_service = QueueService(connection_string=env_connection)
    blob_service = BlockBlobService(connection_string=env_connection)
except Exception as e:
    logger.error(e)
        

def prepare_queue():
    try:
        if not queue_service.exists('jobs'):
            logger.info('creating queue: jobs')
            queue_service.create_queue('jobs')
    except Exception as e:
        logger.error(e)

def handle_message(message):
    try:
        logger.info("handling new message " + message.id )
        job = json.loads(message.content)
        source_name = job["SourceName"]
        style_name = job["StyleName"]
        target_name = job["TargetName"]
        style_weight = job["StyleWeight"]
        size = job["Size"]
        iterations = job["Iterations"]

        image_dir = "/app/images/"
        source_file = os.path.join(image_dir, source_name)
        style_file =  os.path.join(image_dir, style_name)
        out_file =  os.path.join(image_dir, target_name)

        args = ["--content_img", source_file]
        args.extend(["--style_imgs", style_file])
        args.extend(["--content_weight", "1"])
        args.extend(["--style_weight", str(style_weight)])
        args.extend(["--max_size", str(size)])
        args.extend(["--max_iterations", str(iterations)])
        args.extend(["--img_output_dir", image_dir])
        args.extend(["--verbose"])

        logger.info("downloading %s", source_file )
        blob_service.get_blob_to_path("images", source_name, file_path= source_file)

        logger.info("downloading %s", style_file )
        blob_service.get_blob_to_path("images", style_name, file_path= style_file)

        logger.info("start job with Source=%s, Style=%s, Target=%s, Size=%s", source_name, style_name, target_name, size)

        neural_style_calc(args)

        if os.path.exists(out_file):
            logger.info ("uploading file %s", out_file)
            blob_service.create_blob_from_path("results", target_name, out_file)
        else:
            logger.info("file %s does not exit", out_file)

    except Exception as e:
        logger.error(e)

    queue_service.delete_message('jobs', message.id, message.pop_receipt)

def poll_queue():
    try:
        logger.info ("starting to poll jobs queue")
        while True:
            messages = queue_service.get_messages('jobs', num_messages=1, visibility_timeout=10*60)

            if len(messages) > 0:
                handle_message(messages[0])
            
            time.sleep(5)
    except Exception as e:
        logger.error(e)


logger.info ("starting queue client")
prepare_queue()
poll_queue()

