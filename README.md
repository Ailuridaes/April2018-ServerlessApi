λ# - Creating a REST API With the Serverless Framework
======================================================

This challenge is about using the [Serverless Framework](https://serverless.com/framework/) to create a Serverless REST API using API Gateway and Lambda.

Using the serverless framework, will be creating a simple micro bloging platform using only serverless servicess.

Pre-requisites
--------------

<details>
  <summary>Node.js and npm</summary>
  * Download [Node.js and npm](https://www.npmjs.com/get-npm) if you don't have them already.
</details>

<details>
  <summary>Serverless Framework</summary>
  * Download or update to [serverless 1.26+](https://serverless.com/framework/)
  
  ```bash
  npm install serverless -g 
  ```
</details>
<details>
  <summary>.NET Core 2.0 if you plan to use the code provided</summary>
  * Download and install from [.NET Core GitHub repository](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.6-download.md)
  * Install aws lamdba tools (used only for packaging application)
  
  ```bash
  dotnet new -i Amazon.Lambda.Templates::*
  ```
</details>
<details>
  <summary>A valid AWS Account</summary>
  * If you don't have an AWS Account, [create one](https://aws.amazon.com/free).
</details>

<details>
  <summary>Setup the AWS CLI and account profile</summary>
  * [Install the AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/installing.html)  
  * Create an AWS profile 
  ```bash
  aws configure
  ```
</details>
<details>
  <summary>Install Postman (we'll use this for testing)</summary>
 * [Postman | Apps](https://www.getpostman.com/apps) 
</details>

Level 0 - Deploy the API 
------------------------
With serverless is quite simple to deploy your first API! You need a configuration file `serverless.yml` and package with your code to be deployed to lambda. Simply compile+package the code and call the serverless deploy command!

```
dotnet build
dotnet lambda package --configuration release --framework netcoreapp2.0 --output-package bin/release/netcoreapp2.0/deploy-package.zip
serverless deploy
```
<details>
The `serverless deploy` creates a CloudFormation stack. It first creates a deployment bucket used to upload the code package. Once this has been created it uploads the package to the bucket and updates the CloudFormation stack with all the infrastructure required for your API including permissions, lambda functions, triggers, API Gateway and more.  

A conveniende script to wrapp the dotnet calls is also provided so you only have to invoke

```
./build.sh && serverless deploy
```


One the stack has been created, serverless will output information about the service you just created:
```
Serverless: Stack update finished...
Service Information
service: microb-challenge
stage: dev
region: us-east-1
stack: microb-challenge-dev
api keys:
  None
endpoints:
  GET - https://SDFASDF.execute-api.us-east-1.amazonaws.com/dev/item
functions:
  list-microb: microb-challenge-dev-list-microb
```
</details>


Copy and paste the enpoint generated by API Gateway and paste it on your web browser or postman. You should see the following response
```
{"title": "Hello API Gateway!", "content": "foo bar"}
```

Level 1 - Create a DynamoDB Table
---------------------------------
The project contains five functions that need to be implemented: Create, Read, Update, Delete and List.
To implement the create function, we will need to create a DynamoDB table.

### Include the DynamoDB table definition to your servelress.yml file

* Uncomment the `resouces` section in the `serverless.yml` file
  * Under the `resouces` section use [serverless variables](https://serverless.com/framework/docs/providers/aws/guide/variables/) to include `Parameters` and `DynamoDBTable` from the `infrastructure/microb-table_cfn.yml` file.

### Your functions need permissions to interact with the new DynamoDB table
* Uncomment the `iamRoleStatements` section and include the permissions required to do the following actions:
  * put
  * update
  * get
  * delete
  * query 
  * scan

### Add create function configuration
* Under the `functions` section add a new definition for the create function.
* Set the handler to point to the create function
* Set the http method to `post`
* The path should be `item`


### Deploy the updates
* Run `serverless deploy`
* Go to the AWS Console and take a look at the table that was just generated

Level 2 - Create Function
Now that we have the required infrastructure in place, we need to modify the create function so we can start adding items to our application.

### Update the create function to `put` an item in the table
Microbe items contains an `id` and a `title`. You can add any additional information such as `content`, `date`, `author`, and anything else you may want.
Once you are done, re-deploy the function using serverless.

* Put an item to dynamo using a unique Id, an unique Title, Content and any other information you want to include
* Return a `APIGatewayProxyResponse`. You can include a body, headers, a status and other infomration in this object.

<details>
* The APIGatewayProxyRequest object contains information about the API Request when the "Proxy Pass" mode is use in API Gateway
* Properties in the APIGatewayProxyRequest include Body, Headers, Path, Method, PathParameters and more
* The APIGatewayProxyResponse object is transformed by API gateway into a valid HTTP response message
* Typically eventually consistent functions return a 202 status code and a *location* header containing the path to the new resource.
</details>

* To test your API using postman, import the json files included in the `test` directory. 
* Update the `domain` in the microb environment with the URL generated by API Gateway. 

> **HINT**:
> 
> Don't hard code the name of the table in you code, instead pass the value as an environment variable in the serverless configuration.


Level 3 - Read, Update, Delete, List
-------------------------------------
Repete the steps above but for all the other functions  

<details>
* read
* update
* delete 
* list
</details>


Level 4 - Secure your API
----------------------------
Now that your API is up and running we want to make sure that only authorized callers can use it. 

* Use the serverless configuration to add an API key, and secure all API endpoints

Boss Level - Calling your API from a website (Enabling CORS)
---------------------------------------------------------

* Create a static website use the `infrastructure/static-website_cfn.yml`, and include all the the resources into your `serverless.yml` resources section
* [Enable CORS](https://serverless.com/blog/cors-api-gateway-survival-guide/) for your APIs.
* Ensure that your functions return the proper headers
* In the `html` directory, modify the `configuration.js` file
  * Set the correct `api_url`
* Upload the `html` directory to your website bucket
 
