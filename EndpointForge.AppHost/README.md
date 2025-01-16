# EndpointForge.AppHost

<!-- TOC -->
* [EndpointForge.AppHost](#endpointforgeapphost)
  * [Description](#description)
  * [Requirements](#requirements)
  * [Configuration](#configuration)
  * [Testing](#testing)
  * [Building](#building)
  * [Running](#running)
  * [Project References](#project-references)
<!-- TOC -->

## Description

The purpose of this project is to run the `EndpointForge.WebApi` service within the .NET Aspire Environment.  This 
will instantiate the `EndpointForge.WebApi` project as .NET Aspire service with a name of `EndpointForge`, allowing the
distributed application context to be injected into other projects.  As such, this is designed to run as part of a 
non-production environment.  To run in a non-production environment you will need to make the necessary protections 
against malicious actors and denial-of-service attacks.

## Requirements

- .NET 9.0.0 or above.
- Development certificates present on the local machine for HTTPS connections.

_As this project relies on no external dependencies, then there is no requirement for `Docker` or `Podman` to be 
installed to run this project, however, when `EndpointForge` is to be used in external .NET Aspire packages, it will be
required that the hosting machine is running either `Docker` or `Podman`.  The solution level documentation will 
cover this once this feature has been implemented._

To install .NET 9 please see the [.NET Install Documentation](https://learn.microsoft.com/en-us/dotnet/core/install/)
from Microsoft.

To use HTTPS connections when running locally you will need to have local trusted development certificates present.
These can be created and trusted by running the following command:

```shell
  dotnet dev-certs https --trust
```

## Configuration

Default launch profiles for both HTTP and HTTP are provided in the `Properties/launchSettings.json`.

The launch URL and port be changed by modifying the `applicationUrl` property in this file.

The default settings are:
- HTTP: `"http://localhost:15222"`
- HTTPS: `"https://localhost:17016;http://localhost:15222"`

## Testing

There are no units test for this project as the only function is declare the services required to be instantiated in 
the .NET Aspire environment

There are however, a set of integration tests which run against the .NET Aspire environment and are used to test 
functionality of the provided configuration endpoints in the `EndpointForge.WebApi` service using `httpClient` 
requests. 

To run these please see the[Integration Testing Documentation](../EndpointForge.IntegrationTests/README.md) in the 
`EndpointForge.IntegrationTests` project.

## Building

Navigate to the project root and run the following command:

```shell
  dotnet build
```

## Running

To run the project as HTTP, you will need to configure an environment variable for`ASPIRE_ALLOW_UNSECURED_TRANSPORT`.

Please see the
[Aspire AllowUnsecuredTransport Documentation](https://aka.ms/dotnet/aspire/allowunsecuredtransport) for more details.

To run the project as HTTPS, instead run the following:

```shell
  dotnet run
```

This will launch the .NET Aspire environment service and provide a link in the console log to launch the dashboard at 
the configured application urls for the provided profile (HTTPS by default).

Once the service is running, you will be able to use the dashboard to monitor service status, check console logs and 
structured logs, as well as inspect all traces.  Alongside this, as `EndpointForge` is fully .NET Aspire compliant, 
it exposes telemetry and metrics to the .NET Aspire service, allowing these to be viewed with in the dashboard.

Please refer to the 
[.NET Aspire Dashboard Overview](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/overview) 
for more information.


## Project References

- `EndpointForge.WebApi` - Used to add the `EndpointForge.WebApi` project to the .NET Aspire Environment.
