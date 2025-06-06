# Simulated lamp for DomoticASW

## Features
- Control lamp power state (on/off/toggle)
- Adjust brightness with confirm/cancel
- Set color
- Full status monitoring
- Docker container support

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)

## Build and Run

### Without Docker

To build and run the project, just as a test, use the following command:

```bash
dotnet run --project src/Lamp.App/Lamp.App.csproj
```

To run the tests, use the following command:

```bash
dotnet test tests/Lamp.Core.Tests/Lamp.Core.Tests.csproj
```

### With Docker

To build the Docker image and run the container, use the following commands:

```bash
docker build -t smartlamp .
```

Then, to run the container interactively, use:

```bash
docker run -it smartlamp
```
