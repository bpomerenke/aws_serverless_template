output "lambda_role" {
  value = aws_iam_role.lambda_role.id
}

output "lambda_arn" {
  value = aws_lambda_function.lambda.arn
}
output "lambda_function_name" {
  value = aws_lambda_function.lambda.function_name
}

