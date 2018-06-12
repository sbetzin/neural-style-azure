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
        content_weight = job["ContentWeight"]
        size = job["Size"]
        iterations = job["Iterations"]

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
        args.extend(["--verbose"])

        logger.info("downloading %s", source_file )
        blob_service.get_blob_to_path("images", source_name, file_path= source_file)

        logger.info("downloading %s", style_file )
        blob_service.get_blob_to_path("images", style_name, file_path= style_file)

        logger.info("start job with Source=%s, Style=%s, Target=%s, Size=%s", source_name, style_name, target_name, size)

        target_name_origcolor_0 = target_name.replace("#origcolor#", "0")
        target_name_origcolor_1 = target_name.replace("#origcolor#", "1")

        out_file_origcolor_0 =  os.path.join(image_dir, target_name_origcolor_0)
        out_file_origcolor_1 =  os.path.join(image_dir, target_name_origcolor_1)

        neural_style_calc(args)

        upload_file(target_name_origcolor_0, out_file_origcolor_0)
        upload_file(target_name_origcolor_1, out_file_origcolor_1)
    except Exception as e:
        logger.error(e)

    queue_service.delete_message('jobs', message.id, message.pop_receipt)

def upload_file(target_name, file_name):
    try:
        if os.path.exists(file_name):
            logger.info ("uploading file %s", file_name)
            blob_service.create_blob_from_path("results", target_name, file_name)
        else:
            logger.info("file %s does not exit", file_name)
    except Exception as e:
        logger.error(e)
        
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
