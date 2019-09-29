#!/bin/bash
set -e

curl -LO https://www.amazontrust.com/repository/AmazonRootCA1.pem

aws iot create-thing-group --thing-group-name bp-things-group
aws iot create-thing --thing-name "${1}" --thing-type-name bp-things
aws iot add-thing-to-thing-group --thing-group-name bp-things-group --thing-name "${1}"

certificateArn="$(aws iot create-keys-and-certificate --set-as-active \
    --certificate-pem-outfile "${1}"-certificate.pem \
    --private-key-outfile "${1}"-private.pem | jq '.certificateArn' -r)"

aws iot attach-policy --policy-name bp-thing-policy --target ${certificateArn}
aws iot attach-thing-principal --thing-name "${1}" --principal ${certificateArn}

openssl pkcs12 -export -out ${1}.p12 -inkey ${1}-private.pem -in ${1}-certificate.pem