#!/usr/bin/env python

from applicationinsights import TelemetryClient
from applicationinsights.logging import enable

def get_instrumentation_key():
    return '643d7485-b5d7-4b0a-8a35-4dc9107d2dc5'

def enable_logging():
    key = get_instrumentation_key()
    enable(key)

def create_telemetrie_client():
    key = get_instrumentation_key()
    telemetrie = TelemetryClient(key)

    telemetrie.context.operation.name='neural-image-tensorflow'
    telemetrie.channel.sender.send_interval_in_milliseconds = 30 * 1000
    telemetrie.channel.queue.max_queue_length = 10
    
    return telemetrie

#local test
#telemetrie = create_telemetrie_client()
#telemetrie.track_event('Test event')
#telemetrie.track_metric('Test Metric', 42)
#telemetrie.flush()
