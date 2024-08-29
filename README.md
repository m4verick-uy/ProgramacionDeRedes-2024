# Programación de Redes - 2024

### Explicación del Código

- **`Semaphore semaphore = new Semaphore(3, 3);`:**
    - Se crea un `Semaphore` con un contador inicial de 3 y un máximo de 3 permisos. Esto significa que hasta 3 hilos pueden entrar en la sección crítica simultáneamente.

- **`Thread thread = new Thread(AccessResource);`:**
    - Se crean 10 hilos en un bucle, y cada hilo ejecuta el método `AccessResource`.

- **`semaphore.WaitOne();`:**
    - Este método es utilizado por los hilos para solicitar acceso a la sección crítica. Si hay un permiso disponible (es decir, el contador del semaphore es mayor que 0), el hilo procede y el contador del semaphore se decrementa. Si no hay permisos disponibles (contador es 0), el hilo se bloquea hasta que un permiso sea liberado.

- **Sección Crítica:**
    - Entre `semaphore.WaitOne()` y `semaphore.Release()`, se encuentra la sección crítica del código. Aquí, simulamos un trabajo que tarda 1 segundo (`Thread.Sleep(1000)`).

- **`semaphore.Release();`:**
    - Esto libera un permiso en el semaphore, incrementando el contador. Otro hilo que esté esperando podrá entonces entrar en la sección crítica.

- **`Console.ReadLine();`:**
    - Este comando mantiene la consola abierta para que puedas ver la salida hasta que presiones Enter.

### Comparación con `lock`

- **`lock`:**
    - Permite solo un hilo en la sección crítica a la vez.

- **`Semaphore`:**
    - Es más flexible y permite un número específico de hilos acceder simultáneamente a la sección crítica. Esto es útil en escenarios donde se puede permitir un acceso concurrente controlado.


## Autor
#### [**Guillermo Echichure**](mailto:gechichure@gmail.com)
