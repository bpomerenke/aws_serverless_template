resource "aws_api_gateway_rest_api" "api" {
  name               = "Data API"
  description        = "This is the base API"
  binary_media_types = ["multipart/form-data"]
}