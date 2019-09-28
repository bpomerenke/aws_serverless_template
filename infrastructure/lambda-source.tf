resource "aws_s3_bucket" "code_bucket" {
  bucket        = "${var.namespace}-lambda-source"
  region        = "${var.region}"
  force_destroy = true
}

resource "aws_s3_bucket_object" "version_source" {
  bucket = "${aws_s3_bucket.code_bucket.id}"
  key    = "VersionSource"
  source = "../api/artifacts/Version.zip"
  etag   = "${filemd5("../api/artifacts/Version.zip")}"
}

