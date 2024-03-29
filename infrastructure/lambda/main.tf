resource "aws_lambda_function" "lambda" {
  source_code_hash = var.source_hash
  function_name    = var.function_name
  role             = aws_iam_role.lambda_role.arn
  runtime          = "dotnetcore2.1"
  handler          = var.handler
  timeout          = "180"
  memory_size      = var.memory_size
  s3_bucket        = var.source_bucket
  s3_key           = var.source_key

  environment {
    variables = var.variables
  }
}

resource "aws_cloudwatch_log_group" "log" {
  name              = "/aws/lambda/${var.function_name}"
  retention_in_days = 30
}

resource "aws_iam_role" "lambda_role" {
  name = "${var.function_name}-role"

  assume_role_policy = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Principal": {
        "Service": "lambda.amazonaws.com"
      },
      "Action": "sts:AssumeRole"
    }
  ]
} 
EOF
}

resource "aws_iam_role_policy" "log_policy" {
  name = "${var.function_name}-log-policy"
  role = aws_iam_role.lambda_role.id

  policy = <<EOF
{
    "Version": "2012-10-17",
    "Statement": [
        {
          "Effect": "Allow",
          "Action": [
            "logs:CreateLogStream",
            "logs:PutLogEvents"
          ],
          "Resource": "*"
        }
    ]
}
EOF
}

