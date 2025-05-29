# Ejemplo de RabbitMQ Publish/Subscribe - DirecTV IPTV Simulation

Este proyecto simula una señal de cable tipo DirecTV utilizando **RabbitMQ** con el patrón **Publish/Subscribe**. Se implementa en .NET 8 con C# moderno (async/await) y estructura legada (`namespace`, `class`, `Main()`), ideal para uso pedagógico.

## Objetivo

Demostrar cómo RabbitMQ permite publicar mensajes en diferentes "canales" (intereses) y cómo múltiples consumidores pueden suscribirse solo al canal que les interesa, usando **exchange tipo `direct`** y **`routingKey`**.

---

## Estructura

```
Publish-Subscribe/
├── DirectvPublisher/       # Publicador: envía la grilla de canales
│   └── DirectvPublisher.cs
├── DirectvSubscriber/      # Subscriptor: elige a qué canal conectarse
│   └── DirectvSubscriber.cs
└── README.md
```

---

## Ejecución

### 1. Asegurar que RabbitMQ esté corriendo

Ejemplo en Docker:

```bash
docker start rabbitmq
# o si usan docker-compose
docker-compose up -d
```

---

### 2. Ejecutar el Subscriber

```bash
dotnet run --project DirectvSubscriber
```

Verás:

```
Bienvenido a DirecTV - Elija tu canal IPTV:
1 - Noticias
2 - Deportes
3 - Dibujos Animados
Canal:
```

Seleccioná un canal y el subscriber se suscribirá a la cola correspondiente (`noticias`, `deportes` o `dibujos`).

---

### 3. Ejecutar el Publisher

```bash
dotnet run --project DirectvPublisher
```

Esto enviará 6 mensajes por canal con una pequeña demora entre ellos, simulando una transmisión "en vivo". Cada subscriptor recibirá únicamente los mensajes de su canal.

---

## Tecnologías usadas

- .NET 8
- RabbitMQ.Client 7.1.2
- Exchange tipo `direct`
- Routing Keys para segmentar audiencia (`noticias`, `deportes`, `dibujos`)
- Async/await + `AsyncEventingBasicConsumer`

---

## Contenido de los Canales

### Canal Noticias

- Bienvenido al canal NOTICIAS
- Científicos crean plástico biodegradable en 72h
- NASA limpia basura espacial con láseres
- Estudiantes uruguayos ganan mundial de robótica
- Bosques del Amazonas muestran recuperación
- Avance en vacuna contra Alzheimer

### Canal Deportes

- Bienvenido al canal DEPORTES
- Uruguay vs Francia
- Uruguay vs México
- Uruguay vs Ghana (penales)
- Semifinal: Uruguay vs Alemania
- Final: Uruguay vs España

### Canal Dibujos Animados

- Bienvenido al canal DIBUJOS ANIMADOS
- ThunderCats
- Osos Gummy
- Star Wars Lego
- Astroboy
- Street Fighter

---

## Posibles mejoras

- Permitir suscribirse a más de un canal a la vez.
- Usar `fanout` en vez de `direct` para broadcasting general.
- Visualizar en consola las colas desde la interfaz web de RabbitMQ.

---

## Autor

Ejemplo desarrollado para fines didácticos por **Guillermo Echichure** como parte del curso de **Programación de Redes con .NET y RabbitMQ**.

---