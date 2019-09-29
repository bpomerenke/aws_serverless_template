#!/bin/bash
set -e

ENDPOINT="$(aws iot describe-endpoint --endpoint-type iot:Data-ATS | jq '.endpointAddress' -r)"
TOPIC="bp-test/messages"
mosquitto_pub \
    --cafile AmazonRootCA1.pem \
    --cert ${1}-certificate.pem \
    --key ${1}-private.pem \
    -h ${ENDPOINT} \
    -p 8883 -q 1 -d \
    -t ${TOPIC} \
    -i ${1} \
    -m "Hello there"
echo ""
