terraform {
  required_version = "= 0.12.9"

  backend "s3" {
    region  = "us-east-2"
    encrypt = true
  }
}
