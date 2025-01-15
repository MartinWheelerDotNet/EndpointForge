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
        * [Example](#example-2)
          * [AddEndpoint request](#addendpoint-request-)
          * [Created EndpointForge Endpoint Request](#created-endpointforge-endpoint-request)
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
  curl http://localhost:5065/health
```

Expected response: `200(OK) - "Healthy"`

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
  curl http://localhost:5065/alive
```

Response: `200(OK) - "Healthy"`

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
  leading '/' and does not contain the hostname or port details.

* `"methods"`: [ **_string\[\]_** ] - This should contain an array of string values for the HTTP Methods to be used by 
  this endpoint.  The array must not be empty, or contain only whitespace or empty values.

* `Content-Type` Header: [ **_string_** ] - This should always be populated with the value `application\json`
Submitting a POST request containing just a `route` and `methods` fields we result in a default `200 (OK)` response with
an empty response body.

* `Content-Length` Header: [ **_number_** ] - This should be populated with the length of the request body.  `Curl` 
  (when using `--data`)and most other API Clients will auto-populate this field upon sending the request.

##### Example

###### AddEndpoint request 

```shell
  curl --location 'http://localhost:5065/add-endpoint' \
       --header 'Content-Type: application/json' \
       --data '{
          "route": "/endpoint-forge-endpoint-1",
          "methods": [ "GET" ]
       }'
```

Response: `200 (OK)`

###### Created EndpointForge Endpoint Request

```shell
  curl --location 'http://localhost:5065/endpoint-forge-endpoint-1'
```

Response: `200 (OK)`

#### OpenApi Schema
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
                    "statusCode",
                    "message",
                    "errors"
                ],
                "type": "object",
                "properties": {
                    "statusCode": {
                        "$ref": "#/components/schemas/HttpStatusCode"
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
            },
            "HttpStatusCode": {
                "type": "integer"
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



