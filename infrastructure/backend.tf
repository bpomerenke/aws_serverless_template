terraform {
  required_version = "= 0.14.6"

  backend "s3" {
    region  = "us-east-2"
    encrypt = true
  }
}
