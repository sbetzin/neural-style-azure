#!/usr/bin/env python
import datetime
import time
import json
import base64
import os.path
import logging
from azure.storage.queue import QueueService
from azure.storage.blob import BlockBlobService
from algorithms import styletransfer

queue_service = QueueService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')
blob_service = BlockBlobService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

def prepare_queue():
    if not queue_service.exists('jobs'):
        logger.info('creating queue: jobs')
        queue_service.create_queue('jobs')

def handle_message(message):
    try:
        job = json.loads(message.content)
        source_name = job["SourceName"]
        style_name = job["StyleName"]
        target_name = job["TargetName"]
        style_weight = job["StyleWeight"]
        style_scale = job["StyleScale"]
        size = job["Size"]
        iterations = job["Iterations"]
        tile_size = job["TileSize"]
        tile_overlap = job["TileOverlap"]
        use_orig_colors = job["UseOriginalColors"]

        source_file = "/app/images" + source_name + ".jpg"
        style_file = "/app/images/" + style_name + ".jpg"
        out_file = "/app/images/" + target_name + ".jpg"

        blob_service.get_blob_to_path("images", source_name, file_path= source_file)
        blob_service.get_blob_to_path("images", style_name, file_path= style_file)

        print('start job with Source=' + source_name + ', Style='+ style_name + ' to Target=' + target_name)
        styletransfer(source_file, style_file, out_file, size, "gatys", iterations, style_weight, style_scale, tile_size, tile_overlap, use_orig_colors)

        if os.path.exists(out_file):
            logger.info ("uploading file " + out_file)
            blob_service.create_blob_from_path("results", source_name, out_file)
        else:
            logger.info("file " + out_file + " does not exit" )

    except Exception as e:
        logger.error(e)

    queue_service.delete_message('jobs', message.id, message.pop_receipt)

def poll_queue():
    logger.info ("starting to poll jobs queue")
    while True:
        messages = queue_service.get_messages('jobs', num_messages=1, visibility_timeout=10*60)

        if len(messages) > 0:
            handle_message(messages[0])
        
        time.sleep(1)

logger.info ("starting queue client")
prepare_queue()
poll_queue()

