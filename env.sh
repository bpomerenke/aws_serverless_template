export TF_VAR_domain_name=bp-aws-serverless-web-example.com 
export TF_VAR_broker="https://$(aws iot describe-endpoint  | jq '.endpointAddress' -r)"