# Dockerfile.Server (simple, debug)

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .
CMD ["dotnet", "run", "--project", "Server.csproj", "--configuration", "Debug"]
