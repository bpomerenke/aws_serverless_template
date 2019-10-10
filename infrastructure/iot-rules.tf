resource "aws_iot_topic_rule" "message_rule" {
  name        = "Message_Ingestion"
  description = "Message Ingestion Lambda"
  enabled     = true
  sql         = "SELECT clientid() as ClientID, timestamp() as Timestamp, * FROM 'CHAT/Messages'"
  sql_version = "2016-03-23"

  lambda {
    function_arn  = "${module.message_ingestion_lambda.lambda_arn}"
  }
}

# lambda
module "message_ingestion_lambda" {
  source        = "./lambda"
  source_bucket = "${aws_s3_bucket.code_bucket.id}"
  source_key    = "${aws_s3_bucket_object.ingestion_source.id}"
  source_hash   = "${aws_s3_bucket_object.ingestion_source.etag}"
  function_name = "IngestMessage"
  handler       = "Ingestion::Ingestion.Function::IngestMessage"

  variables = {
    Version = "0.3"
  }
}
resource "aws_lambda_permission" "message_ingestion_permission" {
  statement_id  = "AllowExecutionFromIot"
  action        = "lambda:InvokeFunction"
  function_name = "${module.message_ingestion_lambda.lambda_function_name}"
  principal     = "iot.amazonaws.com"
}
