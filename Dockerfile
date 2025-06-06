FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Lamp.Core/Lamp.Core.csproj", "src/Lamp.Core/"]
COPY ["src/Lamp.App/Lamp.App.csproj", "src/Lamp.App/"]

# Restore dependencies
RUN dotnet restore "src/Lamp.App/Lamp.App.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Lamp.App"
RUN dotnet build "Lamp.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Lamp.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lamp.App.dll"]
