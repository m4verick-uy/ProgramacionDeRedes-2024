namespace SemaphoreExample
{
    using System;
    using System.Threading;

    class DbContext
    {
        // Permito un máximo de 2 hilos accediendo a la vez a la zona crítica.
        // (2, 2) -> aforo: el patio arranca ABIERTO con las 2 plazas libres.
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

        public void SaveDataToDatabase(string data)
        {
            _semaphore.Wait(); // Adquiero un cupo del semáforo (bloquea si no hay disponibles)
            try
            {
                Console.WriteLine("Guardando datos: " + data);
                if (data.Contains("error"))
                    throw new Exception("Error al guardar datos");
                Thread.Sleep(1000); // Simulo trabajo de escritura en la base de datos
            }
            finally
            {
                _semaphore.Release(); // Libero el cupo aunque ocurra una excepción
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            var db = new DbContext();

            var t1 = new Thread(() => Ejecutar(db, "Dato1"));
            var t2 = new Thread(() => Ejecutar(db, "error"));
            var t3 = new Thread(() => Ejecutar(db, "Dato2")); // habrá excepción, pero no se bloquea la zona crítica

            t1.Start(); t2.Start(); t3.Start();
            t1.Join(); t2.Join(); t3.Join();

            Console.WriteLine("Operación completada");
            Console.WriteLine("Sigo trabajando en otra cosa ...");
        }

        static void Ejecutar(DbContext db, string dato)
        {
            try
            {
                db.SaveDataToDatabase(dato);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepción capturada en Main: " + ex.Message);
            }
        }
    }
}
