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
resource "aws_lambda_permission" "websocket_connect_permission" {
  function_name = "${module.websocket_connect_lambda.lambda_arn}"
  statement_id  = "AllowExecutionFromApiGateway"
  action        = "lambda:InvokeFunction"
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.region}:${data.aws_caller_identity.current.account_id}:${aws_cloudformation_stack.websocket-api.outputs["GatewayId"]}/*/*"
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
resource "aws_lambda_permission" "websocket_disconnect_permission" {
  function_name = "${module.websocket_disconnect_lambda.lambda_arn}"
  statement_id  = "AllowExecutionFromApiGateway"
  action        = "lambda:InvokeFunction"
  principal     = "apigateway.amazonaws.com"
  source_arn    = "arn:aws:execute-api:${var.region}:${data.aws_caller_identity.current.account_id}:${aws_cloudformation_stack.websocket-api.outputs["GatewayId"]}/*/*"
}

output "ws_url" {
  value = "wss://${aws_cloudformation_stack.websocket-api.outputs["GatewayId"]}.execute-api.${var.region}.amazonaws.com/deployed"
}
