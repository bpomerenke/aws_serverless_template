#!/bin/sh
set -e

export TF_VAR_domain_name=bp-aws-serverless-web-example.com 

cd infrastructure
terraform apply -auto-approve

cd -
aws s3 sync web/dist/web/ s3://${TF_VAR_domain_name} --delete