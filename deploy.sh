#!/bin/sh
set -e

source env.sh

cd infrastructure
terraform apply -auto-approve

cd -
aws s3 sync web/dist/web/ s3://${TF_VAR_domain_name} --delete --exclude "*/appConfig.json"

cd -
REST_API_ID=$(aws apigateway get-rest-apis | jq ".items[]|select(.name | contains(\"Data API\")) | .id" -r)
aws apigateway create-deployment --rest-api-id "${REST_API_ID}" --stage-name="deployed"

WEBSOCKET_API_ID=$(aws apigatewayv2 get-apis | jq ".Items[]|select(.Name | contains(\"Websockets\")) | .ApiId" -r)
aws apigatewayv2 create-deployment --api-id "${WEBSOCKET_API_ID}" --stage-name="deployed"