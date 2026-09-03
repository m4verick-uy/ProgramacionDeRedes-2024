namespace SemaphoreExample
{
    using System;
    using System.Threading;

    class DbContext
    {
        // Pool de 2 conexiones: máximo dos hilos accediendo a la base a la vez.
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

        public void SaveDataToDatabase(string data)
        {
            Console.WriteLine($"[{data}] esperando conexión a la base de datos...");
            _semaphore.Wait(); // pide una conexión; si no hay libres, se bloquea acá
            try
            {
                // Si el aforo funciona, nunca vas a ver más de 2 "ENTRÓ" sin un "salió" en el medio.
                Console.WriteLine($"[{data}] ENTRÓ a la base de datos. Conexiones libres: {_semaphore.CurrentCount}");
                Thread.Sleep(1000); // simula el trabajo de escritura mientras ocupa la conexión
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

            // 6 hilos peleando por 2 conexiones: entran de a 2, en oleadas.
            var hilos = new Thread[6];
            for (int i = 0; i < hilos.Length; i++)
            {
                string dato = $"Dato{i + 1}";
                hilos[i] = new Thread(() => db.SaveDataToDatabase(dato));
            }

            // Le digo al OS que los hilos estan pronto para ser usados.
            foreach (var h in hilos) h.Start();

            // Espero a que todos terminen

            foreach (var h in hilos) h.Join();
            // for (int i = 0; i < hilos.Length; i++)
            // {
            //     hilos[i].Join();
            // }

            Console.WriteLine("Operación completada");
        }
    }
}
