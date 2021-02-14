resource "aws_s3_bucket" "code_bucket" {
  bucket        = "${var.namespace}-lambda-source"
  force_destroy = true
}

resource "aws_s3_bucket_object" "version_source" {
  bucket = aws_s3_bucket.code_bucket.id
  key    = "VersionSource"
  source = "../api/artifacts/Version.zip"
  etag   = filemd5("../api/artifacts/Version.zip")
}
resource "aws_s3_bucket_object" "messages_source" {
  bucket = aws_s3_bucket.code_bucket.id
  key    = "MessagesSource"
  source = "../api/artifacts/Messages.zip"
  etag   = filemd5("../api/artifacts/Messages.zip")
}
resource "aws_s3_bucket_object" "ingestion_source" {
  bucket = aws_s3_bucket.code_bucket.id
  key    = "IngestionSource"
  source = "../api/artifacts/Ingestion.zip"
  etag   = filemd5("../api/artifacts/Ingestion.zip")
}
resource "aws_s3_bucket_object" "websocket_source" {
  bucket = aws_s3_bucket.code_bucket.id
  key    = "WebSocketSource"
  source = "../api/artifacts/WebSocket.zip"
  etag   = filemd5("../api/artifacts/WebSocket.zip")
}

