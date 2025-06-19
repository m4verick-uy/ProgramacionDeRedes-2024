# Dockerfile.Server (simple, debug)
# Si usan Windows o Mac con INTEL este FROM esta bien
# FROM mcr.microsoft.com/dotnet/sdk:8.0

# Si usan Windows o Mac con ARM64 este FROM no les sirve
# Esto es para que funcione en ARM64
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .

RUN mkdir -p /app/images

RUN dotnet restore
RUN dotnet build --configuration Debug

CMD ["dotnet", "run", "--project", "Server.csproj", "--configuration", "Debug"]