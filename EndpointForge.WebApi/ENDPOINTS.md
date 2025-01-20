# EndpointForge.WebApi - Endpoints

## Table of Contents

<!-- TOC -->
* [EndpointForge.WebApi - Endpoints](#endpointforgewebapi---endpoints)
  * [Table of Contents](#table-of-contents)
  * [Description](#description)
  * [System Endpoints](#system-endpoints)
    * [Healthcheck Endpoint](#healthcheck-endpoint)
      * [Responses](#responses)
      * [Example](#example)
    * [Alive Endpoint](#alive-endpoint)
      * [Responses](#responses-1)
      * [Example](#example-1)
  * [Service Endpoints](#service-endpoints)
    * [AddEndpoint Endpoint](#addendpoint-endpoint)
      * [Required Fields](#required-fields)
      * [Required Headers](#required-headers)
        * [Example](#example-2)
      * [Optional fields](#optional-fields)
      * [Error Responses](#error-responses)
      * [Constructing a Response Body](#constructing-a-response-body)
      * [Parameters](#parameters)
        * [Static Parameters](#static-parameters)
        * [Route Values and Query Parameters](#route-values-and-query-parameters)
      * [Placeholders](#placeholders)
      * [Rules](#rules)
        * [Generate Guid Rule](#generate-guid-rule)
          * [Example](#example-3)
        * [Insert Parameter Rule](#insert-parameter-rule)
          * [Static Parameter Example](#static-parameter-example)
          * [Header Parameter Example](#header-parameter-example)
          * [Route Value Example](#route-value-example)
          * [Query Parameter Example](#query-parameter-example)
  * [OpenApi Schema](#openapi-schema)
<!-- TOC -->

## Description

The purpose of this document is to provide information around the various built-in endpoints, their usages and 
possible responses.

## System Endpoints

These endpoints are required for the Aspire Integration and provide information about the readiness of the 
`EndpointForge` service.  They are disabled by default for non-development environments.

If `EndpointForge` is being using a non-development environment you will need to protect these endpoints against 
malicious actors and denial-of-service attacks.  Please refer to the 
[Aspire Fundamentals Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/health-checks#non-development-environments)
for more information on this subject.


### Healthcheck Endpoint

Route: `/health`

Method: `GET`

_When running in a .NET Aspire environment, this will be consumed by the `AppHost` service to provide service status in
the Aspire dashboard._

#### Responses

| Status Code              | Body                     | Description                                                             |
|--------------------------|--------------------------|-------------------------------------------------------------------------|
| 200 (OK)                 | `(text/plain)` Healthy   | The service is running and operational.                                 |
| 200 (OK)                 | `(text/plain)` Degraded  | The service is running slowly or is unstable, a restart is recommended. |
| 503 (Service Unavailable | `(text/plain)` Unhealthy | The service is in a non-functional state, a restart is required.        |

#### Example

```shell
  curl -i http://localhost:5065/health
```

```text
HTTP/1.1 200 OK
Content-Type: text/plain

Healthy
```

<br />

### Alive Endpoint

Route: `/alive`

Method: `GET`

The `Alive Endpoint` mirrors the `Healthcheck Endpoint` as `EndpointForge` relies upon no external 
services need to be confirmed healthy before starting this service.

_When running in a .NET Aspire environment, this will be consumed by the `AppHost` service to provide service status in
the Aspire dashboard._

#### Responses

| Status Code              | Body                      | Description                                                             |
|--------------------------|---------------------------|-------------------------------------------------------------------------|
| 200 (OK)                 | `(text/plain)` Healthy    | The service is running and operational.                                 |
| 200 (OK)                 | `(text/plain)` Degraded   | The service is running slowly or is unstable, a restart is recommended. |
| 503 (Service Unavailable | `(text/plain)` Unhealthy  | The service is in a non-functional state, a restart is required.        |

#### Example

```shell
  curl -i http://localhost:5065/alive
```

```text
HTTP/1.1 200 OK
Content-Type: text/plain

Healthy
```

<br/>

## Service Endpoints

These endpoints are used for the management and creation of dynamic endpoints in the `EndpointForge` service.


### AddEndpoint Endpoint

Route: `/add-endpoint`

Method: `POST`

The `AddEndpoint` endpoint is used to create new dynamic `EndpointForge` endpoints.

The request body should be of type `application/json` and provides the configuration and responses provided by the 
`EndpointForge` service.  The details of the request body are as below:

#### Required Fields

* `"route"`: [ **_string_** ] - This should contain a valid route for the endpoint, route can start with or without a 
  leading `/` and does not contain the hostname or port details.

* `"methods"`: [ **_string\[\]_** ] - This should contain an array of string values for the HTTP Methods to be used by 
  this endpoint.  The array must not be empty, or contain only whitespace or empty values.

#### Required Headers

* `Content-Type` Header: [ **_string_** ] - This should always be populated with the value `application\json`

* `Content-Length` Header: [ **_number_** ] - This should be populated with the length of the request body.  `Curl` 
  (when using `--data`), .NET `HttpClient` and most other API Clients will autopopulate this field upon sending the 
  request.

The response will be `201 (Created)` if the required fields and headers were set, and the `Location` header will be set
to the path of the created resource.


##### Example

An example `Add Endpoint Request` containing just the `route` and `methods` fields.

<H6>_Add Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/add-endpoint' \
          --header 'Content-Type: application/json' \
          --data '{
              "route": "/required-fields",
              "methods": [ "GET" ]
          }'
```

```text
HTTP/1.1 201 Created
Location: /required-fields
```

<H6>_Created Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/required-fields'
```

```text
HTTP/1.1 200 OK
```


#### Optional fields

* `"response"` [ **_Object_**] - This should be used to configure the response required from the newly created endpoint.

if a response is provided the following field may be set on the `response` object:

* `"response.statusCode"` - This is the status code to be returned by the response, by default this value will be 
  `200`. If provided this must be populated with a valid HTTP Status Code.

* `"contentType"` - This is the content-type of the response.  This will populate the `Content-Type` header with 
    the provided value.  If this is not provided, then the response `Content-Type` will be either 
    `appplication/json` for a json body response, `text/plain` for a simple string or will remain unpopulated for an 
    empty response body.

* `"body"` - This is the body of the response. By default, this will be an empty request body if this is not 
    provided. Constructing the response body is explored in detail in the 
    [Constructing A Response Body](#constructing-a-response-body) section below.

* `"parameters"` - This should be used to provide a list of parameter names and values to be used when constructing 
  the response through the use of `Placeholders`.  `Parameters` and `Placeholders` are explored more in the 
  [Parameters and Placeholders](#parameters) section below.

<br />

#### Error Responses

| Status Code                 | Body          | Error Status Code         | Description                                                                |
|-----------------------------|---------------|---------------------------|----------------------------------------------------------------------------|
| 400 (Bad Request)           | ErrorResponse | INVALID_REQUEST_BODY      | Request body was of an unknown type, empty, or is missing required fields. |
| 409 (Conflict)              | ErrorResponse | ROUTE_CONFLICT            | Request contains one or more route conflicts.                              |
| 422 (Unprocessable Entity)  | ErrorResponse | REQUEST_BODY_INVALID_JSON | Request contains invalid JSON body which cannot be processed.              |
| 500 (Internal Server Error) | ErrorResponse | UNKNOWN_SERVER_ERROR      | An unhandled internal server error has occured.                            |

If the request was not successful, an error response will be returned with the relevant Error Status Code and 
message. It will also contain an array of Error strings, which will detail the reason for the failure.

The `ErrorResponse` will be in the following format:

```json
{
  "statusCode": "<ERROR_STATUS_CODE",
  "message": "<ERROR_MESSAGE>",
  "errors": [
    "<AN_ERROR_REASON>",
    "<ANOTHER_ERROR_REASON>"
  ]
}
```

The response headers will also contain a header with the key `X-Trace-Id`.  This can you used when searching 
structured logs to identify this request.

<br/>

#### Constructing a Response Body

When constructing a response body, a templated approach is used where you can add `rules` to request via 
`placeholders`.  These allow the response body to insert data captured from the request, duplicate sections of the 
provided body, and generate certain types of values such as GUIDs.

**This is currently implemented with the minimum of rules and placeholders and this section will expand over time.**

#### Parameters

`Parameters` allow the providing of fixed values, or the capturing of values from the request body of the 
request being made to created endpoint.

To add `parameters` into the `Add Endpoint Request` then the optional property `parameters` is used.  This contains 
an array of `Parameter` objects, which use the structure below:

* `"type"`: This is the type of parameter being used and must be one of the following values:
* * "static"
* * "header"
* `"name"`: This is the parameter name, and is used when in a `placeholder` to reference the provided `value`
* `"value"`:  This is the value of the parameter and is used by a `placeholder` when inserting parameters.

```json
{
  "parameters": [
    {
      "type": "string",
      "name": "string",
      "value": "string"
    }
  ]
}
```

##### Static Parameters

`Parameters` with a type of `"static"` are used to store fixed values to be inserted into the response body.

<H6>Available Rules</H6>

* [Insert Parameter Rule](#insert-parameter-rule)

`Parameters` with a type of `"header"` are used to capture header values from the request calling an
`EndpointForge` endpoint created by an `Add Endpoint Request`.

<H6>Available Rules</H6>

* [Insert Parameter Rule](#insert-parameter-rule)

##### Route Values and Query Parameters

Route values and query parameters are automatically captured and available as static parameters.

For example if a route of `/route-parameters/{identifier}?id=id-value&name=name-value` was specified in the `Add 
Endpoint Request`, then static parameters of `identifier`, `id` and `name` will add with values captured from 
the request route.

<H6>Available Rules</H6>

* [Insert Parameter Rule](#insert-parameter-rule)

#### Placeholders

`Placeholders` are used when constructing a response to tell the `EndpointForge` service to replace the value with 
the results of the specified rule.  A `placeholder` is indicated by surrounding a rule instruction double curly 
braces.

#### Rules

A `Rule` is a placeholder included in the response body.  A `Rule` placeholder is made up of segments and all use 
the following pattern:

```text
{{instruction:type:value}}
````

* `instruction`: The action that will be completed by this rule. This must be one of the following values:
* * `generate`: A rule which generates a value each time the placeholder is encountered in the provided 
response body template.
* * `insert`: A rule which inserts a captured parameter value.
* `type`: Indicates the type of rule, for instance a `generate` rule might provide a `type` of `guid` for a guid 
value to be generated
* `value`: Indicates a value provided for to this rule, and can take a variety of uses and may be omitted by certain 
rules.

<br/>
 
##### Generate Guid Rule

Placeholder: `{{generate:guid}}` or `{{generate:guid:<parameter name>}}`
Details: This rule is used to generate a unique guid value each time the placeholder is encountered. When a 
parameter name is provided, the generated value with stored as parameter and available for use in other rules.

###### Example

<H6>_Add Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/add-endpoint' \
          --header 'Content-Type: application/json' \
          --data '{
            "route": "/generate-guid-rule",
            "methods": [ "GET" ],
            "response": {
              "statusCode": 200,
              "contentType": "application/json",
              "body": "1: {{generate:guid}}. 2: {{generate:guid:guid-capture}}. 3: {{insert:parameter:guid-capture}}"
            }
          }'
```

```text
HTTP/1.1 201 Created
Location: /generate-guid-rule
```

<H6>_Created Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/generate-guid-rule'
```

```text
HTTP/1.1 200 OK
Content-Length: 100
Content-Type: application/json

1: 3c5a3f1c-a0db-43df-a974-b3e06bbfd755. 2: 9cedb34a-4c09-4dad-8491-cac91c17e0ae. 3: 9cedb34a-4c09-4dad-8491-cac91c17e0ae

```

As you can see, each time the generate guid rule placeholder was encountered, a different unique guid is generated to 
replace that placeholder. When the second guid included a parameter, the insert parameter rule inserted that 
captured parameter, resulting in `2:` and `3:` being the same guid.

<br/>

##### Insert Parameter Rule

Placeholder: `{{insert:parameter:<parameter name>}}`.
Details:  This rule is used to insert a captured parameter in to the response body by replacing the provided 
placeholder with the value of parameter named in the `<parameter name>` segment above.
 
###### Static Parameter Example

In the example below, a `"static"` parameter named, `parameter-name` is added the `Add Endpoint Request` with a
value of`parameter-value`.  The response body contains a matching insert parameter rule.


<H6>_Add Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/add-endpoint' \
          --header 'Content-Type: application/json' \
          --data '{
            "route": "/insert-parameter-rule-static",
            "methods": [ "GET" ],
            "response": {
              "statusCode": 200,
              "contentType": "application/json",
              "body": "Static parameter value: {{insert:parameter:parameter-name}}"
            },
            "parameters": [
              {
                "type": "static",
                "name": "parameter-name",
                "value": "parameter-value"
              }
            ]
          }'
```

```text
HTTP/1.1 201 Created
Location: /insert-parameter-rule-static
```

<H6>_Created Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/insert-parameter-rule-static'
```

```text
HTTP/1.1 200 OK
Content-Length: 39
Content-Type: application/json

Static parameter value: parameter-value
```

In the response above, you can see that the placeholder has been replaced with the value of the static parameter 
`parameter-name` with was provided in the `Add Endpoint Request`.

<br/>

###### Header Parameter Example

In the example below, a `"header"` parameter named `XCustom-Header` is added to the `Add Endpoint Request` with a
value of `header-parameter`.  The response body contains a matching insert parameter rule.

<H6>_Add Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/add-endpoint' \
          --header 'Content-Type: application/json' \
          --data '{
            "route": "/insert-parameter-rule-header",
            "methods": [ "GET" ],
            "response": {
              "statusCode": 200,
              "contentType": "application/json",
              "body": "XCustom-Header value: {{insert:parameter:header-parameter}}"
            },
            "parameters": [
              {
                "type": "header",
                "name": "XCustom-Header",
                "value": "header-parameter"
              }
            ]
          }'
```

```text
HTTP/1.1 201 Created
Location: /insert-parameter-rule-header
```

<H6>_Created Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/insert-parameter-rule-header' \
          --header 'XCustom-Header: header-value'
```

```text
HTTP/1.1 200 OK
Content-Length: 34
Content-Type: application/json

XCustom-Header value: header-value
```

In the response above, you can see that the placeholder has been replaced with the value of the header `XCustom-Header` 
provided in the `Curl` request.

<br/>

###### Route Value Example

In the example below, a route is provided with a route value named `route-value-identifier`. When calling the endpoint 
created by an `Add Endpoint Request` with a route value then this is captured as a parameter value.  The response body 
value of the`Add Endpoint Request` contains a matching insert static parameter rule.

<H6>_Add Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/add-endpoint' \
          --header 'Content-Type: application/json' \
          --data '{
            "route": "/insert-parameter-rule-route/{route-value-identifier}",
            "methods": [ "GET" ],
            "response": {
              "statusCode": 200,
              "contentType": "application/json",
              "body": "Identifier value: {{insert:parameter:route-value-identifier}}"
            }
          }'
```

```text
HTTP/1.1 201 Created
Location: /insert-parameter-rule-route/{route-value-identifier}

```

<H6>_Created Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/insert-parameter-rule-route/identifier-value'
```

```text
HTTP/1.1 200 OK
Content-Length: 34
Content-Type: application/json

Identifier value: identifier-value
```

In the response above, you can see that the placeholder has been replaced with the route value which was provided 
when calling the endpoint created by the `Add Endpoint Request`.

<br/>

###### Query Parameter Example

In the example below, when calling the endpoint created by the `Add Endpoint Request` and providing query parameters 
of `id=id-value` and `name=name-value` then these are captured for use by the rules.  The response body value of the 
`Add Endpoint Request` contains a matching insert static parameter rule for each of the query parameters.

<H6>_Add Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/add-endpoint' \
          --header 'Content-Type: application/json' \
          --data '{
            "route": "/insert-parameter-rule-query",
            "methods": [ "GET" ],
            "response": {
              "statusCode": 200,
              "contentType": "application/json",
              "body": "Id value: {{insert:parameter:id}}. Name value: {{insert:parameter:name}}"
            }
          }'
```

```text
HTTP/1.1 201 Created
Location: /insert-parameter-rule-query
```

<H6>_Created Endpoint Request_</H6>

```shell
  curl -i --location 'http://localhost:5065/insert-parameter-rule-query?id=id-value&name=name-value'
```

```text
HTTP/1.1 200 OK
Content-Length: 42
Content-Type: application/json

Id value: id-value. Name value name-value
```

In the response above, you can see that the placeholders has been replaced with corresponding query parameter value 
which was provided when calling the endpoint created by the `Add Endpoint Request`.

<br/>

## OpenApi Schema
<Details>
<summary>Schema</summary>

```json
{
  "openapi": "3.0.1",
  "info": {
    "title": "EndpointForge.WebApi | v1",
    "version": "1.0.0"
  },
  "servers": [
    {
      "url": "http://localhost:5065"
    }
  ],
  "paths": {
    "/add-endpoint": {
      "post": {
        "tags": [
          "EndpointForge.WebApi"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/AddEndpointRequest"
              }
            }
          },
          "required": true
        },
        "responses": {
          "201": {
            "description": "Created"
          },
          "400": {
            "description": "Bad Request",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ErrorResponse"
                }
              }
            }
          },
          "409": {
            "description": "Conflict",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ErrorResponse"
                }
              }
            }
          },
          "422": {
            "description": "Unprocessable Entity",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ErrorResponse"
                }
              }
            }
          },
          "500": {
            "description": "Internal Server Error",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ErrorResponse"
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "AddEndpointRequest": {
        "required": [
          "route",
          "methods"
        ],
        "type": "object",
        "properties": {
          "route": {
            "type": "string"
          },
          "methods": {
            "type": "array",
            "items": {
              "type": "string"
            }
          },
          "response": {
            "type": "object",
            "properties": {
              "statusCode": {
                "type": "integer",
                "format": "int32"
              },
              "contentType": {
                "type": "string",
                "nullable": true
              },
              "body": {
                "type": "string",
                "nullable": true
              }
            }
          },
          "parameters": {
            "type": "array",
            "items": {
              "required": [
                "type",
                "name",
                "value"
              ],
              "type": "object",
              "properties": {
                "type": {
                  "type": "string"
                },
                "name": {
                  "type": "string"
                },
                "value": {
                  "type": "string"
                }
              }
            }
          }
        }
      },
      "ErrorResponse": {
        "required": [
          "errorStatusCode",
          "message",
          "errors"
        ],
        "type": "object",
        "properties": {
          "errorStatusCode": {
            "type": "string"
          },
          "message": {
            "type": "string"
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          }
        }
      }
    }
  },
  "tags": [
    {
      "name": "EndpointForge.WebApi"
    }
  ]
}
```

</Details>



