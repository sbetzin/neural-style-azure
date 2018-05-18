#!/usr/bin/env python
import datetime
import time
import json
import base64
from azure.storage.queue import QueueService

queue_service = QueueService(account_name='neuralstylefiles', account_key='mMxv0dYg1xyEqE5VsrZejnH1PKQL5NsvG2gwYAfyHCrN1LDGYTXztCLoyfXa7ObB9BpPvXhGBtBg2A6owaV3gQ==')


def prepare_queue():
    if not queue_service.exists('jobs'):
        print('creating queue: jobs')
        queue_service.create_queue('jobs')

def handle_message(message):
    #print(message.id + ' - ' + message.content + ' - ' + message.pop_receipt)

    try:
        job = json.loads(message.content)
        source = job["source"]
        data = base64.b64decode(source)
        image = open("test.jpg","wb")
        image.write(data)
        image.close

        print('job id=' + job["id"])
    except Exception as e:
        print(e)

    queue_service.delete_message('jobs', message.id, message.pop_receipt)



def poll_queue():
    #queue_service.put_message('jobs', u'{"id":"test","source":"abc"}')

    while True:
        messages = queue_service.get_messages('jobs', num_messages=1, visibility_timeout=10*60)

        if len(messages) > 0:
            handle_message(messages[0])
        
        time.sleep(1)

prepare_queue()
poll_queue()

