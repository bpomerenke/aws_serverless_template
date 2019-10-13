resource "aws_s3_bucket" "web_s3" {
  bucket        = "${var.domain_name}"
  acl           = "public-read"
  force_destroy = "true"

  policy = "${data.template_file.web_s3_policy.rendered}"

  website {
    index_document = "index.html"
    error_document = "index.html"
  }
}

data "template_file" "web_s3_policy" {
  template = "${file("web-hosting-policy.tpl.json")}"

  vars = {
    bucket_name = "${var.domain_name}"
  }
}

output "s3_web_url" {
    value = "${aws_s3_bucket.web_s3.website_endpoint}"
}
