# Imagen base con SDK para compilar y ejecutar
FROM mcr.microsoft.com/dotnet/sdk:8.0

# Carpeta de trabajo dentro del contenedor
WORKDIR /app

# Copiamos todo el contenido del proyecto al contenedor
COPY . .

# Restauramos paquetes y compilamos
RUN dotnet restore
RUN dotnet build --configuration Debug

# Ejecutamos el proyecto gRPC
CMD ["dotnet", "run", "--project", "GrpcServer.csproj", "--configuration", "Debug"]

