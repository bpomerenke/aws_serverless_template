variable "source_bucket" {
  type = "string"
}

variable "source_key" {
  type = "string"
}

variable "source_hash" {
  type = "string"
}
variable "function_name" {
  type = "string"
}

variable "handler" {
  type = "string"
}

variable "http_method" {
  type = "string"
}

variable "variables" {
  type = "map"
  default = {}
}

variable "region" {
  type = "string"
}

variable "account_id" {
  type = "string"
}

variable "rest_api_id" {
  type = "string"
}

variable "resource_id" {
  type = "string"
}

variable "memory_size" {
  type    = "string"
  default = "1024"
}

variable "include_cors" {
  type    = "string"
  default = "1"
}
