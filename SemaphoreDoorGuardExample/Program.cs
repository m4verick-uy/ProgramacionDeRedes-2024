namespace SemaphoreDoorGuardExample
{
    using System;
    using System.Threading;

    class DbContext
    {
        // Arranca CERRADO: 0 disponibles de 2. Nadie entra hasta que el portero abra.
        // (0, 2) -> señalización: el patio existe (cap. 2) pero arranca sin plazas libres.
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0, 2);

        // El portero llama esto para liberar plazas desde afuera (la "V" que destranca).
        public void AbrirPlazas(int cantidad)
        {
            _semaphore.Release(cantidad);
        }

        public void SaveDataToDatabase(string data)
        {
            Console.WriteLine($"[{data}] esperando conexión a la base de datos...");
            _semaphore.Wait(); // se bloquea acá: no hay plazas hasta que el portero abra
            try
            {
                Console.WriteLine($"[{data}] ENTRÓ a la base de datos. Conexiones libres: {_semaphore.CurrentCount}");
                Thread.Sleep(2000); // simula el trabajo mientras ocupa la conexión
                Console.WriteLine($"[{data}] salió de la base de datos.");
            }
            finally
            {
                _semaphore.Release(); // libera la conexión (siempre, aun si hubo excepción)
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var db = new DbContext();

            // 6 hilos que quieren entrar, pero el patio arranca cerrado (0 plazas).
            var hilos = new Thread[6];
            for (int i = 0; i < hilos.Length; i++)
            {
                string dato = $"Dato{i + 1}";
                hilos[i] = new Thread(() => db.SaveDataToDatabase(dato));
            }

            // Le aviso al OS que los 6 hilos están prontos: todos van a quedar esperando.
            foreach (var h in hilos) h.Start();

            // Portero: espera 3 segundos con el patio cerrado y recién ahí abre las 2 plazas.
            var portero = new Thread(() =>
            {
                Console.WriteLine(">>> Portero: el patio está cerrado, todos esperan...");
                Thread.Sleep(3000);
                Console.WriteLine(">>> Portero: ¡abro las 2 plazas!");
                db.AbrirPlazas(2); // libera 2 cupos -> empiezan a entrar de a 2
            });
            portero.Start();

            // Espero a que todos terminen
            foreach (var h in hilos) h.Join();
            portero.Join();

            Console.WriteLine("Operación completada");
        }
    }
}
