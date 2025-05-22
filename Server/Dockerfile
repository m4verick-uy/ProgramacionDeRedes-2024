# Dockerfile.Server (simple, debug)

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .

RUN mkdir -p /app/images

RUN dotnet restore
RUN dotnet build --configuration Debug

CMD ["dotnet", "run", "--project", "Server.csproj", "--configuration", "Debug"]