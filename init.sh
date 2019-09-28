#!/bin/sh
set -e

cd infrastructure
terraform init -backend-config=backend_configs/prodev -reconfigure