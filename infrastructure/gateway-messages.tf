
resource "aws_api_gateway_resource" "messages_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.api.id}"
  parent_id   = "${aws_api_gateway_rest_api.api.root_resource_id}"
  path_part   = "messages"
}

module "messages_get_lambda" {
  source        = "./api-lambda"
  source_bucket = "${aws_s3_bucket.code_bucket.id}"
  source_key    = "${aws_s3_bucket_object.messages_source.id}"
  source_hash   = "${aws_s3_bucket_object.messages_source.etag}"
  function_name = "GetMessages"
  handler       = "Messages::Messages.Function::FunctionHandler"
  http_method   = "GET"
  region        = "${var.region}"
  account_id    = "${data.aws_caller_identity.current.account_id}"
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.messages_resource.id}"

  variables = {
    CORSAllowedOrigin = "*"
  }
}

