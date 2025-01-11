# EndpointForge.WebApi

The purpose of this project is to provide a high performance REST based API to allow configuration, 
creation, and management of EndpointForge dynamic endpoints, whilst keeping memory allocation as low as possible.

This project will also serve as the host server for all EndpointForge APIs created.

This document covers building this project as a standalone service.
For more details about building and running as part of the .NET Aspire service, please see the 
[Aspire AppHost Documentation](../EndpointForge.AppHost/README.md#running) in the `EndpointForge.AppHost` project for more details.

## Requirements

- .NET 9.0.0 or above.
- Development certificates present on the local machine for HTTPS connections.

To install .NET 9 please see the [Install .NET Documentation](https://learn.microsoft.com/en-us/dotnet/core/install/) from Microsoft.

To use HTTPS connections when running locally you will need to have local trusted development certificates present.
These can be created and trusted by running the following command:

```shell
  dotnet dev-certs https --trust
```

## Configuration

Default launch profiles for both HTTP and HTTP are provided in the `Properties/launchSettings.json`.

The launch URL and port be changed by modifying the `applicationUrl` property in this file.

The default settings are:
- HTTP: `"http://localhost:5065"`
- HTTPS: `"http://localhost:5065;https://localhost:7074"`

## Testing

To run the unit tests for this project, please see the [Testing Documentation](../EndpointForge.WebApi.Tests/README.md) in the `EndpointForge.WebApi.Tests` 
project.

There are also a set of integration tests which run in the Aspire environment and are used to test functionality of 
the provided configuration endpoints using `httpClient` requests.  To run these please see the
[Integration Testing Documentation](../EndpointForge.IntegrationTests/README.md) in the `EndpointForge.IntegrationTests`
project.

## Building

Navigate to the project root and run the following command:

```shell
  dotnet build
```

## Running

To run the project as HTTP only, navigate to the project root and run the following command:

```shell
  dotnet run
```

To run the project as HTTPS, then run the following:

```shell
  dotnet run --launchProfile https
```

This will launch the service and will run on the configured application urls for the provided profile (HTTP by default).

Once the service is running, you will be able to use the provided REST endpoints to manage endpoints, and to call any 
newly created APIs.

## Endpoints

Detailed documentation on each of the provided REST API endpoints can be found in the [ENDPOINTS.md](./ENDPOINTS.md) 
file.

## Project References

- `EndpointForge.ServiceDefaults` - Used to configure telemetry and metrics, required for running in a .NET Aspire 
environment.
- `EndpointForge.Core` - Defines the interfaces, contracts, extensions and models for EndpointForge.
