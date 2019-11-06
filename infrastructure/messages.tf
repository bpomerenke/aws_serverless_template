
# dynamo table
resource "aws_dynamodb_table" "messages_table" {
  name         = "messages"
  billing_mode = "PAY_PER_REQUEST"

  hash_key = "ClientId"
  range_key = "Timestamp"

  attribute {
    name = "ClientId"
    type = "S"
  }

  attribute {
    name = "Timestamp"
    type = "S"
  }

  stream_enabled = true
  stream_view_type = "NEW_IMAGE"
}

resource "aws_lambda_event_source_mapping" "messages_event_source_mapping" {
  event_source_arn  = "${aws_dynamodb_table.messages_table.stream_arn}"
  function_name     = "${module.messages_notifications_trigger.lambda_arn}"
  starting_position = "LATEST"
}

module "messages_notifications_trigger" {
  source        = "./lambda"
  source_bucket = "${aws_s3_bucket.code_bucket.id}"
  source_key    = "${aws_s3_bucket_object.messages_source.id}"
  source_hash   = "${aws_s3_bucket_object.messages_source.etag}"
  function_name = "NotifyMessageUpdate"
  handler       = "Messages::Messages.Function::NotifyMessageUpdate"

  variables = {
    Version = "0.3"
    WebSocketConnectionsTableName = "${aws_dynamodb_table.websocket_connections_table.id}"
    MQTTBroker        = "${var.broker}"
    ServiceURL        = "https://${aws_cloudformation_stack.websocket-api.outputs["GatewayId"]}.execute-api.${var.region}.amazonaws.com/deployed"
  }
}

resource "aws_iam_role_policy" "messages_notifications_dynamo_policy" {
  name = "${module.messages_notifications_trigger.lambda_function_name}-dynamo-policy"
  role = "${module.messages_notifications_trigger.lambda_role}"

  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [        {
            "Action": [
                "dynamodb:GetRecords",
                "dynamodb:GetShardIterator",
                "dynamodb:DescribeStream",
                "dynamodb:ListStreams"
            ],
            "Resource": [
                "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.messages_table.id}/stream/*"
            ],
            "Effect": "Allow"
        },        
        {
          "Effect": "Allow",
          "Action": [
            "dynamodb:DescribeTable",
            "dynamodb:Scan",
            "dynamodb:GetItem",
            "dynamodb:Query"
          ],
          "Resource": [
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.websocket_connections_table.id}",
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.websocket_connections_table.id}/index/*"
          ]
        },
        {
            "Action": [
                "execute-api:*"           
            ],
            "Effect": "Allow",
            "Resource": [
                "arn:aws:execute-api:${var.region}:${data.aws_caller_identity.current.account_id}:*/*/*"
            ]
        }
    ]
}
EOF
}
