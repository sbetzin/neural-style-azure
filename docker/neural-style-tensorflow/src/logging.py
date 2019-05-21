#!/usr/bin/env python

from applicationinsights import TelemetryClient

tc = TelemetryClient('643d7485-b5d7-4b0a-8a35-4dc9107d2dc5')
tc.track_event('Test event')
tc.flush()