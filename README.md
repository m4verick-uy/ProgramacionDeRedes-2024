# RabbitMQ Example

Este proyecto demuestra el uso básico de RabbitMQ en .NET 8, utilizando dos aplicaciones de consola: un publicador (`Publisher`) y un suscriptor (`Subscriber`).

## Estructura del Proyecto

- **Publisher**: Este proyecto se encarga de enviar mensajes a una cola de RabbitMQ.
- **Subscriber**: Este proyecto escucha y recibe mensajes de la misma cola.

## Descripción del Código

### Publisher

La clase `Publisher` en el proyecto `Publisher` realiza las siguientes tareas:

1. **Establece una conexión con RabbitMQ**: Utiliza `ConnectionFactory` para conectarse al servidor RabbitMQ que está en ejecución en `localhost`.
  
2. **Declara la cola**: Se declara una cola llamada `simple_queue` con las siguientes características:
   - **Durable**: `false` (no sobrevivirá a reinicios).
   - **Exclusive**: `false` (puede ser usada por otras conexiones).
   - **AutoDelete**: `false` (no se eliminará automáticamente).
  
3. **Envía mensajes**: Envía 5 mensajes a la cola, cada uno con el texto `"Hello World {i}"`, donde `{i}` es un número del 0 al 4. Después de enviar cada mensaje, espera 1 segundo.

### Subscriber

La clase `Subscriber` en el proyecto `Subscriber` realiza las siguientes tareas:

1. **Establece una conexión con RabbitMQ**: Similar al publicador, utiliza `ConnectionFactory` para conectarse al servidor RabbitMQ en `localhost`.

2. **Declara la cola**: Declara la misma cola `simple_queue` con las mismas características que el publicador.

3. **Escucha mensajes**: Configura un consumidor que espera mensajes en la cola. Cuando se recibe un mensaje, se imprime en la consola con el texto `"Received: {message}"`.

## Diagrama de Flujo

```plaintext
 +---------------------+      +---------------------+
 |                     |      |                     |
 |     Publisher       |      |     Subscriber      |
 |                     |      |                     |
 +---------------------+      +---------------------+
            |                          |
            |   1. Enviar mensaje      |
            | ------------------------>|
            |                          |
            |                          | 2. Recibir mensaje
            |                          |<-------------------------
            |                          |

