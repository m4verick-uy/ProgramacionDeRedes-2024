## Monitor 

### Uso correcto de monitor

```csharp
Monitor.Enter(lockObject);
try
{
    // Código crítico
}
finally
{
    Monitor.Exit(lockObject);
}
```

### Ejemplo sin `Monitor` (noMonitorExample)

- **Salida no controlada:** Los hilos ejecutan el método `SaveDataToDatabase` simultáneamente sin coordinación, lo que lleva a que los mensajes de "Guardando datos" puedan intercalarse, y los hilos no respetan un orden de ejecución. Esto puede causar que la salida sea impredecible, dependiendo del orden en que los hilos lleguen a la consola.

- **Excepción sin manejar:** Cuando uno de los hilos genera una excepción (en este caso, cuando el dato contiene "error"), no hay sincronización para evitar que otros hilos entren en la sección crítica. Como resultado, mientras un hilo está imprimiendo "Guardando datos: error", otro hilo podría también intentar imprimir su propio mensaje.

### Sincronización con `Monitor`

- **Uso de `Monitor.Enter` y `Monitor.Exit`:** Aquí, `Monitor.Enter` y `Monitor.Exit` se utilizan para asegurar que solo un hilo a la vez pueda ejecutar la sección crítica de código, es decir, el bloque que está dentro de `Monitor.Enter` y `Monitor.Exit`.

- **Ejecución ordenada de hilos:** Debido a la sincronización, un hilo debe esperar a que el hilo que actualmente está en la sección crítica termine su trabajo (libere el bloqueo) antes de poder acceder a ella. Esto asegura que los mensajes "Guardando datos: ..." se impriman de manera secuencial y no intercalada.

- **Problema de excepción sin `try-finally`:** Aunque la sincronización asegura la ejecución ordenada, si ocurre una excepción, `Monitor.Exit` no se ejecutará, dejando el `lockObject` bloqueado. Esto significa que si un hilo lanza una excepción, los demás hilos nunca podrán acceder a la sección crítica porque el bloqueo no se liberó. Como no hay un `try-finally`, `Monitor.Exit` no se ejecuta y otros hilos quedan esperando indefinidamente, o no pueden entrar nunca a la sección crítica.

### Problemas al no usar `try-finally` con `Monitor`

No usar un bloque `try-finally` alrededor de `Monitor.Enter` y `Monitor.Exit` puede llevar a varios problemas serios en el manejo de la sincronización:

1. **Pérdida del bloqueo (Deadlock o interbloqueo):**
   Si no usas `try-finally`, y ocurre una excepción después de que hayas llamado a `Monitor.Enter` pero antes de que se llame a `Monitor.Exit`, el hilo nunca llegará a ejecutar `Monitor.Exit`. Esto significa que el objeto bloqueado permanecerá bloqueado indefinidamente, y ningún otro hilo podrá acceder a la región crítica que depende de ese bloqueo. Esto puede llevar a un interbloqueo, donde los hilos quedan esperando por siempre a que se libere el recurso.

2. **Bloqueo de otros hilos (Inanición o starvation):**
   Si el bloqueo no se libera debido a la ausencia de `Monitor.Exit`, otros hilos que intenten entrar en la región crítica quedarán bloqueados indefinidamente, lo que puede llevar a un estado de inanición donde algunos hilos no pueden progresar porque no pueden adquirir el bloqueo.

3. **Inconsistencia en los datos:**
   Al no liberar el bloqueo correctamente, los recursos compartidos que están protegidos por el bloqueo pueden quedar en un estado inconsistente. Esto es especialmente problemático si otros hilos dependen de la consistencia de esos datos para sus propias operaciones.

### Problema de no usar el objeto de bloqueo (`lockObject`)

- **Sin objeto de bloqueo:** Si no usas un objeto de bloqueo específico (`lockObject`) para sincronizar el acceso a la sección crítica, puedes enfrentar problemas similares a los descritos anteriormente. Sin un objeto de bloqueo, no hay una referencia única que los hilos puedan utilizar para coordinar el acceso a la sección crítica, lo que puede llevar a problemas de concurrencia y datos inconsistentes.

- **Acceso concurrente no controlado:** Sin un objeto de bloqueo, múltiples hilos pueden acceder a la sección crítica al mismo tiempo sin ninguna coordinación, lo que puede causar interacciones inesperadas entre los hilos y resultados impredecibles en la salida o en el estado de los datos.


## Autor
#### [**Guillermo Echichure**](mailto:gechichure@gmail.com)
