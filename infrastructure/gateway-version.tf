
resource "aws_api_gateway_resource" "version_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.api.id}"
  parent_id   = "${aws_api_gateway_rest_api.api.root_resource_id}"
  path_part   = "version"
}

module "version_lambda" {
  source        = "./api-lambda"
  source_bucket = "${aws_s3_bucket.code_bucket.id}"
  source_key    = "${aws_s3_bucket_object.version_source.id}"
  source_hash   = "${aws_s3_bucket_object.version_source.etag}"
  function_name = "GetVersion"
  handler       = "Version::Version.Function::FunctionHandler"
  http_method   = "GET"
  region        = "${var.region}"
  account_id    = "${data.aws_caller_identity.current.account_id}"
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"

  variables = {
    Version = "0.3"
    CORSAllowedOrigin = "*"
  }
}

