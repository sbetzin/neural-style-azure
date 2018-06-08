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
from neural_style import main

env_connection = os.environ['AzureStorageConnectionString']

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

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
        job = json.loads(message.content)
        source_name = job["SourceName"]
        style_name = job["StyleName"]
        target_name = job["TargetName"]
        style_weight = job["StyleWeight"]
        # style_scale = job["StyleScale"]
        size = job["Size"]
        iterations = job["Iterations"]
        # tile_size =  env_tile_size if env_tile_size is not None else job["TileSize"]
        # tile_overlap = job["TileOverlap"]
        # use_orig_colors = job["UseOriginalColors"]

        args = ["--content_img", source_name]
        args.extend(["--style_imgs", style_name])
        args.extend(["--content_weight", "1"])
        args.extend(["--style_weight", style_weight])
        args.extend(["--max_size", size])
        args.extend(["--max_iterations", iterations])
        args.extend(["--device","/cpu:0"])
        args.extend(["--img_output_dir", "/app/images/"])
        args.extend(["--verbose"])

        source_file = "/app/images/" + source_name
        style_file = "/app/images/" + style_name
        out_file = "/app/images/" + target_name

        blob_service.get_blob_to_path("images", source_name, file_path= source_file)
        blob_service.get_blob_to_path("images", style_name, file_path= style_file)

        print('start job with Source=' + source_name + ', Style='+ style_name + ', Target=' + target_name + ', Size=' + size)

        main(args)

        if os.path.exists(out_file):
            logger.info ("uploading file " + out_file)
            blob_service.create_blob_from_path("results", target_name, out_file)
        else:
            logger.info("file " + out_file + " does not exit" )

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

