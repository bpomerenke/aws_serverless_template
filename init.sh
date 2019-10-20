#!/bin/sh
set -e

source env.sh

cd infrastructure
terraform init -backend-config=backend_configs/prodev -reconfigure