# Programación de Redes - 2024

# 🧪 Guía de Laboratorio
## Ejemplo Cliente/Servidor en .NET con Docker y RabbitMQ

---

## ✅ ¿Por qué funciona nuestro ejemplo en .NET?
Funciona porque cliente y servidor comparten entorno, pero Docker **aísla los contenedores por defecto**, y `127.0.0.1` ya no representa el mismo “espacio de red”.

---

## 🧱 Paso a paso

### 1. Crear una red Docker virtual
Esto permite que los contenedores se comuniquen por nombre:

```bash
docker network create red-local-2025
```

---

### 2. Construir las imágenes (en una PC nueva o entorno limpio)
```bash
cd Server
docker build -t server-image .

cd ../Client
docker build -t client-image .
```

---

### 3. ¿Qué contiene esta imagen?

- Sistema base **Debian 12 Slim**
- **.NET 8 SDK** y runtime
- Proyecto `Server` con:
    - `Server.cs`, `Server.csproj`, carpeta `images/`
- Ejecuta con:
  ```json
  CMD ["dotnet", "run", "--project", "Server.csproj", "--configuration", "Debug"]
  ```

---

### 4. ¿Qué no contiene?

- No tiene GUI, ni init system, ni servicios innecesarios.
- No instala .NET en tu máquina.
- No requiere Visual Studio ni Rider.

> 🧠 "Estamos creando un Linux chiquito y liviano que sabe compilar y correr .NET 8..."

---

### 5. Ejecutar el servidor conectado a la red

```bash
docker run -it --rm --name server-container --network red-local-2025 server-image
```

- `--rm`: elimina el contenedor al detenerlo
- `--name`: define un nombre resoluble por DNS en la red Docker

---

### 6. Modificar código del Cliente

**Antes:**
```csharp
socketCliente.Connect(IPAddress.Parse("127.0.0.1"), 10000);
```

**Ahora:**
```csharp
socketCliente.Connect("server-container", 10000);
```

> .NET resuelve el nombre usando `getaddrinfo()` gracias al DNS interno de Docker.

---

### 7. Ejecutar el cliente

```bash
docker run -it --rm --name client-container --network red-local-2025 client-image
```

Para múltiples clientes:

```bash
docker run -it --rm --name client2-container --network red-local-2025 client-image
```

---

## 🔄 Resumen

✅ Contenedores separados  
✅ Comunicación por red Docker  
✅ `IPAddress.Any` en el servidor  
✅ Nombre de contenedor como DNS  
✅ Múltiples clientes concurrentes  
✅ Dockerfile simple

---

## 🧪 Cliente en PC y Server Dockerizado

1. El server toma IP `0.0.0.0` → escucha en todas las interfaces.
2. El client corre **fuera de Docker**, pero se conecta a la IP real del host.
3. Se expone el puerto del contenedor hacia afuera:

```bash
docker run -it --rm -p 10000:10000 --name server-container server-image
```

**App.config Server:**
```xml
<add key="ServerIP" value="0.0.0.0" />
<add key="ServerPort" value="10000" />
```

**App.config Cliente:**
```xml
<add key="ServerIP" value="192.168.x.y" />
```

---

## 📁 Carpeta `images/` mapeada desde el contenedor

### Requisitos:
- Crear carpeta `images` dentro del proyecto
- Darle permisos:
    - Linux/macOS: `chmod -R 777 images/`
    - Windows: `icacls "ruta" /grant Everyone:(OI)(CI)F /T`

### Código en el server:
```csharp
string imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "images");
string fileName = $"mensaje_{DateTime.Now.Ticks}.txt";
string filePath = Path.Combine(imagesDir, fileName);
Directory.CreateDirectory(imagesDir);
await File.WriteAllTextAsync(filePath, mensaje);
```

### Ejecutar:
```bash
docker run -it --rm -v $(pwd)/images:/app/images -p 10000:10000 --name server-container server-image
```

---

## 🧩 Server como librería de clases + gRPC

1. Cambiar tipo de proyecto: de consola → Class Library
2. Quitar `Main()`, crear `StartOldServer()`
3. Crear proyecto gRPC
4. Referenciar librería `OldServer` desde gRPC
5. Llamar `StartOldServer()` antes de `Run()`

```csharp
OldServer.StartOldServer();
app.Run();
```

---

### 🐳 Dockerfile de gRPC Server

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet build --configuration Debug
CMD ["dotnet", "run", "--project", "GrpcServer.csproj", "--configuration", "Debug"]
```

---

### 📦 Migrar App.config

Mover `App.config` a la raíz de `GrpcServer`, ya que el framework lo espera ahí.

---

## 📦 docker-compose.yml final con RabbitMQ

```yaml
version: '3.9'

services:
  grpc-server:
    build:
      context: ./GrpcServer
      platform: linux/amd64
    image: grpc-server-image
    container_name: grpc-server-container
    networks:
      - red-local-2025
    tty: true
    stdin_open: true
    ports:
      - "5001:5001"
      - "10000:10000"
    volumes:
      - ./GrpcServer/OldServer/images:/app/images

  rabbitmq:
    image: rabbitmq:4-management
    container_name: rabbitmq-container
    ports:
      - "5672:5672"
      - "15672:15672"
    networks:
      - red-local-2025
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest

networks:
  red-local-2025:
    driver: bridge
```

---

## 🧪 Para ejecutar todo

```bash
docker-compose down
docker-compose build --no-cache
docker-compose up
```


## Autor
#### [**Guillermo Echichure**](mailto:gechichure@gmail.com)
