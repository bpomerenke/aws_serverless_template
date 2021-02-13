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
* create thing (see next section)
* Run in simulator

## Create certificates (register devices)
* `cd manual_scripts/`
* `./create_thing.sh test03` 
* set the password to `123` (hardcoded in iOS app)
   * NOTE: if creating more than 1 you can use whatever name you want, you just have to change iOS code to look for a different cert `line 18: let clientID = "test03"`
   * Once the `.p12` file is created, create a relative link inside iOS project under `dummy_iot` folder

## creating new lambdas
* `cd api`
* `dotnet new lambda.EmptyFunction --name <function name>`
* `dotnet sln api.sln add <function name>/src/<function name>/<function name>.csproj`
* `dotnet sln api.sln add <function name>/test/<function name>.Tests/<function name>.Tests.csproj`