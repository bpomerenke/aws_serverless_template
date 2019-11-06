# Lambda
module "lambda" {
  source        = "../lambda"
  source_bucket = "${var.source_bucket}"
  source_key    = "${var.source_key}"
  source_hash   = "${var.source_hash}"
  function_name = "${var.function_name}"
  handler       = "${var.handler}"
  variables     = "${var.variables}"
}

resource "aws_lambda_permission" "permission" {
  function_name = "${module.lambda.lambda_arn}"
  statement_id  = "AllowExecutionFromApiGateway"
  action        = "lambda:InvokeFunction"
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.region}:${var.account_id}:${var.rest_api_id}/*/*"
}

# API Gateway
resource "aws_api_gateway_integration" "integration" {
  rest_api_id             = "${var.rest_api_id}"
  resource_id             = "${var.resource_id}"
  http_method             = "${var.http_method}"
  type                    = "AWS_PROXY"
  uri                     = "arn:aws:apigateway:${var.region}:lambda:path/2015-03-31/functions/${module.lambda.lambda_arn}/invocations"
  integration_http_method = "POST"
  passthrough_behavior    = "WHEN_NO_TEMPLATES"
}
resource "aws_api_gateway_method" "method" {
  rest_api_id   = "${var.rest_api_id}"
  resource_id   = "${var.resource_id}"
  authorization = "NONE"
  http_method   = "${var.http_method}"

  request_parameters = {
    "method.request.header.ContentType" = true
    "method.request.header.Accept"      = true
  }
}

resource "aws_api_gateway_method_response" "response" {
  rest_api_id   = "${var.rest_api_id}"
  resource_id   = "${var.resource_id}"
  http_method   = "${aws_api_gateway_method.method.http_method}"
  status_code   = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = true
  }
}

# OPTIONS (CORS)
resource "aws_api_gateway_method" "options_method" {
  rest_api_id   = "${var.rest_api_id}"
  resource_id   = "${var.resource_id}"
  http_method   = "OPTIONS"
  authorization = "NONE"
  count         = "${var.include_cors}"
}

resource "aws_api_gateway_method_response" "options_200" {
  rest_api_id   = "${var.rest_api_id}"
  resource_id   = "${var.resource_id}"
  http_method   = "${aws_api_gateway_method.options_method.*.http_method[0]}"
  status_code   = "200"
  count         = "${var.include_cors}"

  response_models = {
    "application/json" = "Empty"
  }

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = true
    "method.response.header.Access-Control-Allow-Methods" = true
    "method.response.header.Access-Control-Allow-Origin"  = true
  }

  depends_on = ["aws_api_gateway_method.options_method"]
}

resource "aws_api_gateway_integration" "options_integration" {
  rest_api_id   = "${var.rest_api_id}"
  resource_id   = "${var.resource_id}"
  http_method   = "${aws_api_gateway_method.options_method.*.http_method[0]}"
  type          = "MOCK"
  count         = "${var.include_cors}"

  request_templates = {
    "application/json" = <<EOF
{
   "statusCode": 200
}
EOF
  }

  depends_on = ["aws_api_gateway_method.options_method"]
}

resource "aws_api_gateway_integration_response" "options_integration_response" {
  rest_api_id   = "${var.rest_api_id}"
  resource_id   = "${var.resource_id}"
  http_method   = "${aws_api_gateway_method.options_method.*.http_method[0]}"
  status_code   = "${aws_api_gateway_method_response.options_200.*.status_code[0]}"
  count         = "${var.include_cors}"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,OPTIONS,POST,PUT,DELETE'"
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
  }

  depends_on = ["aws_api_gateway_method_response.options_200"]
}


