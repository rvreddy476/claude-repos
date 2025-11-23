FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and project files
COPY ChatSystem.sln .
COPY src/ChatSystem.Domain/ChatSystem.Domain.csproj src/ChatSystem.Domain/
COPY src/ChatSystem.Application/ChatSystem.Application.csproj src/ChatSystem.Application/
COPY src/ChatSystem.Infrastructure/ChatSystem.Infrastructure.csproj src/ChatSystem.Infrastructure/
COPY src/ChatSystem.API/ChatSystem.API.csproj src/ChatSystem.API/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /source/src/ChatSystem.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ChatSystem.API.dll"]
