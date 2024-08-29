# Programación de Redes - 2024

## lock

### Uso correcto de lock

```csharp
lock (lockObject)
{
    // Código crítico
}
```

### Sincronización con `lock`

- **Uso de `lock`:** El keyword `lock` en C# simplifica el manejo de bloqueos al encapsular automáticamente la adquisición y liberación del bloqueo. Esto asegura que solo un hilo a la vez pueda ejecutar la sección crítica de código, evitando problemas de sincronización y mejorando la legibilidad del código.

- **Ejecución ordenada de hilos:** Debido a la sincronización proporcionada por `lock`, un hilo debe esperar a que el hilo que actualmente está en la sección crítica termine su trabajo (libere el bloqueo) antes de poder acceder a ella. Esto garantiza que los mensajes "Guardando datos: ..." se impriman de manera secuencial y no intercalada.

- **Manejo de Excepciones:** `lock` maneja automáticamente la liberación del bloqueo, incluso si ocurre una excepción dentro del bloque `lock`. Esto evita problemas como el interbloqueo o la inanición, y asegura que el recurso compartido se libere adecuadamente.


## Autor
#### [**Guillermo Echichure**](mailto:gechichure@gmail.com)
