resource "aws_s3_bucket" "web_s3" {
  bucket        = var.domain_name
  acl           = "public-read"
  force_destroy = "true"

  policy = data.template_file.web_s3_policy.rendered

  website {
    index_document = "index.html"
    error_document = "index.html"
  }
}

data "template_file" "web_s3_policy" {
  template = file("web-hosting-policy.tpl.json")

  vars = {
    bucket_name = var.domain_name
  }
}
data "template_file" "web_appConfig" {
  template = file("web-appConfig.tpl.json")

  vars = {
    apiBaseUrl = "https://${aws_api_gateway_rest_api.api.id}.execute-api.${var.region}.amazonaws.com/deployed"
    wsBaseUrl  = "wss://${aws_cloudformation_stack.websocket-api.outputs["GatewayId"]}.execute-api.${var.region}.amazonaws.com/deployed"
  }
}

resource "aws_s3_bucket_object" "appConfig" {
  bucket       = aws_s3_bucket.web_s3.id
  key          = "assets/data/appConfig.json"
  content      = data.template_file.web_appConfig.rendered
  etag         = "$${data.template_file.web_appConfig.rendered.md5}"
  content_type = "application/json"
}


output "s3_web_url" {
    value = aws_s3_bucket.web_s3.website_endpoint
}
