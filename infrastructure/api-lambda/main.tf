module "lambda" {
  source        = "../lambda"
  source_bucket = "${var.source_bucket}"
  source_key    = "${var.source_key}"
  source_hash   = "${var.source_hash}"
  function_name = "${var.function_name}"
  handler       = "${var.function_name}"
  variables     = "${var.variables}"
}

resource "aws_lambda_permission" "permission" {
  function_name = "${module.lambda.lambda_arn}"
  statement_id  = "AllowExecutionFromApiGateway"
  action        = "lambda:InvokeFunction"
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.region}:${var.account_id}:${var.rest_api_id}/*/*"
}

resource "aws_api_gateway_integration" "integration" {
  rest_api_id             = "${var.rest_api_id}"
  resource_id             = "${var.resource_id}"
  http_method             = "${var.http_method}"
  type                    = "AWS_PROXY"
  uri                     = "arn:aws:apigateway:${var.region}:lambda:path/2015-03-31/functions/${module.lambda.lambda_arn}/invocations"
  integration_http_method = "POST"
  passthrough_behavior    = "WHEN_NO_TEMPLATES"
}


