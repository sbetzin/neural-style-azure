#!/usr/bin/env python
from applicationinsights import TelemetryClient
from applicationinsights import channel
from applicationinsights.channel import AsynchronousQueue
from applicationinsights.channel import AsynchronousSender
from applicationinsights.logging import enable

def get_instrumentation_key():
    return '643d7485-b5d7-4b0a-8a35-4dc9107d2dc5'

def enable_logging():
    key = get_instrumentation_key()
    enable(key)

def create_telemetrie_client():
    key = get_instrumentation_key()

    #https://microsoft.github.io/ApplicationInsights-Python/
    queue = AsynchronousQueue(AsynchronousSender())
    telemetrie_channel = channel.TelemetryChannel(None, queue)
    telemetrie = TelemetryClient(key, telemetry_channel = telemetrie_channel)

    # flush telemetry if we have 10 or more telemetry items in our queue
    telemetrie.channel.queue.max_queue_length = 10
    # send telemetry to the service in batches of 5
    telemetrie.channel.sender.send_buffer_size = 1
    # the background worker thread will be active for 5 seconds before it shuts down. if
    # during this time items are picked up from the queue, the timer is reset.
    telemetrie.channel.sender.send_time = 5
    # the background worker thread will poll the queue every 0.5 seconds for new items
    telemetrie.channel.sender.send_interval = 0.5

    telemetrie.context.operation.name = 'neural-image-tensorflow'

    return telemetrie

#local test
#telemetrie = create_telemetrie_client()
#telemetrie.track_event('Test event')
#telemetrie.track_metric('Test Metric', 42)
#telemetrie.flush()
