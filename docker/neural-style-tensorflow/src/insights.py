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
    client = TelemetryClient(key)

    client.context.operation.name='neural-image-tensorflow'

    return client


telemetrie = create_telemetrie_client()

telemetrie.track_event('Test event')
telemetrie.track_metric('Test Metric', 42)
telemetrie.flush()
