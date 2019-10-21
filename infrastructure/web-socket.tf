resource "aws_cloudformation_stack" "websocket-api" {
  name          = "WebsocketApi"
  template_body = "${file("./cf/websocket-api.yml")}"
  capabilities  = ["CAPABILITY_NAMED_IAM", "CAPABILITY_AUTO_EXPAND"]

  parameters = {
    ConnectFunctionArn    = "${module.websocket_connect_lambda.lambda_arn}"
    DisconnectFunctionArn = "${module.websocket_disconnect_lambda.lambda_arn}"
  }
}


module "websocket_connect_lambda" {
  source        = "./lambda"
  source_bucket = "${aws_s3_bucket.code_bucket.id}"
  source_key    = "${aws_s3_bucket_object.websocket_source.id}"
  source_hash   = "${aws_s3_bucket_object.websocket_source.etag}"
  function_name = "WebSocketConnect"
  handler       = "WebSocket::WebSocket.Function::Connect"

  variables = {
    Version = "0.3"
  }
}
module "websocket_disconnect_lambda" {
  source        = "./lambda"
  source_bucket = "${aws_s3_bucket.code_bucket.id}"
  source_key    = "${aws_s3_bucket_object.websocket_source.id}"
  source_hash   = "${aws_s3_bucket_object.websocket_source.etag}"
  function_name = "WebSocketDisconnect"
  handler       = "WebSocket::WebSocket.Function::Disconnect"

  variables = {
    Version = "0.3"
  }
}

output "ws_url" {
  value = "https://${aws_cloudformation_stack.websocket-api.outputs["GatewayId"]}.execute-api.${var.region}.amazonaws.com/deployed"
}
