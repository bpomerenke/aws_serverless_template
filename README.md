# aws_serverless_template

This project gets the nuts and bolts of a terraformed serverless example up and running

# prerequisites
* Terraform
   * `brew install terraform`
* AWS CLI
* Dotnet core
   * https://dotnet.microsoft.com/download
   * `dotnet new -i Amazon.Lambda.Templates`
   * `dotnet tool install -g Amazon.Lambda.Tools`

# Editor / plugins
* Visual Studio Code
  * Terraform

# dev workflow
* `export AWS_PROFILE=<profile>`
* `./init.sh`
* `./deploy.sh`
* (OPTIONAL) `./teardown.sh`

## creating new lambdas
* `cd api`
* `dotnet new lambda.EmptyFunction --name <function name>`
* `dotnet sln api.sln add <function name>/src/<function name>/<function name>.csproj`
* `dotnet sln api.sln add <function name>/test/<function name>.Tests/<function name>.Tests.csproj`