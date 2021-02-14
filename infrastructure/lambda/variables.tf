variable "source_bucket" {
  type = string
}

variable "source_key" {
  type = string
}

variable "source_hash" {
  type = string
}
variable "function_name" {
  type = string
}

variable "handler" {
  type = string
}

variable "variables" {
  type = map
  default = {}
}

variable "memory_size" {
  type    = string
  default = "1024"
}
