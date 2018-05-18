#!/usr/bin/env python
import datetime
import time
import json
import base64
from azure.storage.queue import QueueService
from azure.storage.blob import BlockBlobService
from neuralstyle.algorithms import styletransfer

queue_service = QueueService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')
blob_service = BlockBlobService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')

def prepare_queue():
    if not queue_service.exists('jobs'):
        print('creating queue: jobs')
        queue_service.create_queue('jobs')

def handle_message(message):
    try:
        job = json.loads(message.content)
        sourceId = job["Source"]
        styleId = job["Style"]

        blob_service.get_blob_to_path("images", sourceId, file_path= "/app/images/source.jpg")
        blob_service.get_blob_to_path("images", styleId, file_path= "/app/images/style.jpg")

        print('start job with SourceId=' + sourceId + ', StyleId='+ styleId)
        styletransfer(["/app/images/source.jpg"], ["/app/images/style.jpg"], "app/images/out.jpg", 1200, "gatys", 500, [50], [1], 1700, 100, [1], None)

    except Exception as e:
        print(e)

    queue_service.delete_message('jobs', message.id, message.pop_receipt)

def poll_queue():
    while True:
        messages = queue_service.get_messages('jobs', num_messages=1, visibility_timeout=10*60)

        if len(messages) > 0:
            handle_message(messages[0])
        
        time.sleep(1)

prepare_queue()
poll_queue()

