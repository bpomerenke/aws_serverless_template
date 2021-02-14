resource "aws_iot_thing_type" "bp_things" {
  name = "bp-things"
}

resource "aws_iot_policy" "cert_pubsub_policy" {
  name = "bp-thing-policy"

  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
    {
        "Action": [
            "iot:Connect",
            "iot:Publish",
            "iot:Receive",
            "iot:Subscribe"
        ],
        "Effect": "Allow",
        "Resource": "arn:aws:iot:*:*:*"
    }]
}
EOF
}

data "aws_iot_endpoint" "custom_endpoint" {}

output "iot_endpoint" {
  value = data.aws_iot_endpoint.custom_endpoint.endpoint_address
}
