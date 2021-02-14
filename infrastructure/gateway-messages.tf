
resource "aws_api_gateway_resource" "messages_resource" {
  rest_api_id = aws_api_gateway_rest_api.api.id
  parent_id   = aws_api_gateway_rest_api.api.root_resource_id
  path_part   = "messages"
}

module "messages_get_lambda" {
  source        = "./api-lambda"
  source_bucket = aws_s3_bucket.code_bucket.id
  source_key    = aws_s3_bucket_object.messages_source.id
  source_hash   = aws_s3_bucket_object.messages_source.etag
  function_name = "GetMessages"
  handler       = "Messages::Messages.Function::GetMessages"
  http_method   = "GET"
  region        = var.region
  account_id    = data.aws_caller_identity.current.account_id
  rest_api_id   = aws_api_gateway_rest_api.api.id
  resource_id   = aws_api_gateway_resource.messages_resource.id

  variables = {
    MessagesTableName = aws_dynamodb_table.messages_table.id
    CORSAllowedOrigin = "*"
    MQTTBroker        = var.broker
  }
}
resource "aws_iam_role_policy" "messages_dynamo_policy" {
  name = "${module.messages_get_lambda.lambda_function_name}-dynamo-policy"
  role = module.messages_get_lambda.lambda_role

  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
          "Effect": "Allow",
          "Action": [
            "dynamodb:DescribeTable",
            "dynamodb:Scan",
            "dynamodb:GetItem"
          ],
          "Resource": [
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.messages_table.id}",
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.messages_table.id}/index/*",
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.websocket_connections_table.id}",
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.websocket_connections_table.id}/index/*"
          ]
        }
    ]
}
EOF
}

module "messages_post_lambda" {
  source        = "./api-lambda"
  source_bucket = aws_s3_bucket.code_bucket.id
  source_key    = aws_s3_bucket_object.messages_source.id
  source_hash   = aws_s3_bucket_object.messages_source.etag
  function_name = "PostMessage"
  handler       = "Messages::Messages.Function::PostMessage"
  http_method   = "POST"
  region        = var.region
  account_id    = data.aws_caller_identity.current.account_id
  rest_api_id   = aws_api_gateway_rest_api.api.id
  resource_id   = aws_api_gateway_resource.messages_resource.id
  include_cors  = 0

  variables = {
    MessagesTableName = aws_dynamodb_table.messages_table.id
    CORSAllowedOrigin = "*"
    MQTTBroker        = var.broker
  }
}
resource "aws_iam_role_policy" "messages_post_dynamo_policy" {
  name = "${module.messages_post_lambda.lambda_function_name}-dynamo-policy"
  role = module.messages_post_lambda.lambda_role

  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
          "Effect": "Allow",
          "Action": [
            "dynamodb:DescribeTable",
            "dynamodb:UpdateItem",
            "dynamodb:PutItem"
          ],
          "Resource": [
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.messages_table.id}",
            "arn:aws:dynamodb:${var.region}:${data.aws_caller_identity.current.account_id}:table/${aws_dynamodb_table.messages_table.id}/index/*"
          ]
        },
        {
            "Effect": "Allow",
            "Action": [
                "iot:Connect",
                "iot:Publish",
                "iot:Subscribe",
                "iot:Receive",
                "iot:GetThingShadow",
                "iot:UpdateThingShadow",
                "iot:DeleteThingShadow"
            ],
            "Resource": "*"
        }

    ]
}
EOF
}

