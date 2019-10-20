#!/bin/sh

if [ "${TF_VAR_domain_name}" == "" ]; then
    echo "_____________________________________________________________________________________"
    echo ""
    echo " !!! Please initialize this environment (e.g. source env.sh) !!! "
    echo "_____________________________________________________________________________________"
    echo ""
    exit 1
fi

mkdir -p src/assets/data/
echo "getting appconfig from ${TF_VAR_domain_name}..."
aws s3 cp s3://${TF_VAR_domain_name}/assets/data/appConfig.json src/assets/data/appConfig.json
