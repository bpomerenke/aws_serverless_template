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
* jq
   * `brew install jq`
* mosquitto (for testing)
   * `brew install mosquitto`
* Cocoapods (for `pod install`)
   * `sudo gem install cocoapods`

# Editor / plugins
* Visual Studio Code
  * Terraform

# dev workflow
## Web app
* `cd web`
* `npm install`
* `npm start`
* open browser to http://localhost:4200

## Infrastructure
* `export AWS_PROFILE=<profile>`
* `./init.sh`
* `./build.sh`
* `./deploy.sh`
* (OPTIONAL) `./teardown.sh`

## iOS app
* `cd dummy_iot`
* `pod install`
* open in xcode
* update `line 19: let defaultHost = "ab5bhz2ubggz4-ats.iot.us-east-2.amazonaws.com"` to be the iot url from terraform `deploy.sh` output variable `iot_endpoint`
* Run in simulator

## creating new lambdas
* `cd api`
* `dotnet new lambda.EmptyFunction --name <function name>`
* `dotnet sln api.sln add <function name>/src/<function name>/<function name>.csproj`
* `dotnet sln api.sln add <function name>/test/<function name>.Tests/<function name>.Tests.csproj`