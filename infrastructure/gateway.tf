resource "aws_api_gateway_rest_api" "api" {
  name               = "Data API"
  description        = "This is the base API"
  binary_media_types = ["multipart/form-data"]
}

resource "aws_api_gateway_resource" "version_resource" {
  rest_api_id = "${aws_api_gateway_rest_api.api.id}"
  parent_id   = "${aws_api_gateway_rest_api.api.root_resource_id}"
  path_part   = "version"
}

resource "aws_api_gateway_method" "method" {
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"
  authorization = "NONE"
  http_method   = "GET"

  request_parameters = {
    "method.request.header.ContentType" = true
    "method.request.header.Accept"      = true
  }
}

resource "aws_api_gateway_method_response" "response" {
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"
  http_method   = "${aws_api_gateway_method.method.http_method}"
  status_code   = "200"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Origin" = true
  }
}

# OPTIONS (CORS)
resource "aws_api_gateway_method" "options_method" {
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"
  http_method   = "OPTIONS"
  authorization = "NONE"
}

resource "aws_api_gateway_method_response" "options_200" {
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"
  http_method   = "${aws_api_gateway_method.options_method.http_method}"
  status_code   = "200"

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
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"
  http_method   = "${aws_api_gateway_method.options_method.http_method}"
  type          = "MOCK"

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
  rest_api_id   = "${aws_api_gateway_rest_api.api.id}"
  resource_id   = "${aws_api_gateway_resource.version_resource.id}"
  http_method   = "${aws_api_gateway_method.options_method.http_method}"
  status_code   = "${aws_api_gateway_method_response.options_200.status_code}"

  response_parameters = {
    "method.response.header.Access-Control-Allow-Headers" = "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'"
    "method.response.header.Access-Control-Allow-Methods" = "'GET,OPTIONS,POST,PUT,DELETE'"
    "method.response.header.Access-Control-Allow-Origin"  = "'*'"
  }

  depends_on = ["aws_api_gateway_method_response.options_200"]
}
