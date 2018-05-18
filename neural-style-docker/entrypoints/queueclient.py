#!/usr/bin/env python
import datetime
import time
import json
import base64
import os.path
import logging
from azure.storage.queue import QueueService
from azure.storage.blob import BlockBlobService
from neuralstyle.algorithms import styletransfer

queue_service = QueueService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')
blob_service = BlockBlobService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')

logging.basicConfig(level=logging.INFO)
LOGGER = logging.getLogger(__name__)

def prepare_queue():
    if not queue_service.exists('jobs'):
        print('creating queue: jobs')
        queue_service.create_queue('jobs')

def handle_message(message):
    try:
        job = json.loads(message.content)
        sourceId = job["Source"]
        styleId = job["Style"]
        sizes = job["Sizes"]

        source_file = "/app/images" + sourceId + ".jpg"
        style_file = "/app/images/" + styleId + ".jpg"
        out_file = "/app/images/" + sourceId + "_out.jpg"

        blob_service.get_blob_to_path("images", sourceId, file_path= source_file)
        blob_service.get_blob_to_path("images", styleId, file_path= style_file)

        print('start job with SourceId=' + sourceId + ', StyleId='+ styleId)
        styletransfer([source_file], [style_file], out_file, sizes, "gatys", [500], [50.0], [1.0], 1700, 100, [1], None)

        if os.path.exists(out_file):
            print ("uploading file " + out_file)
            blob_service.create_blob_from_path("results", sourceId, out_file)
        else:
            print("file " + out_file + " does not exit" )

    except Exception as e:
        print(e)

    queue_service.delete_message('jobs', message.id, message.pop_receipt)

def poll_queue():
    print ("starting to poll jobs queue")
    while True:
        messages = queue_service.get_messages('jobs', num_messages=1, visibility_timeout=10*60)

        if len(messages) > 0:
            handle_message(messages[0])
        
        time.sleep(1)

print ("starting queue client")
prepare_queue()
poll_queue()

