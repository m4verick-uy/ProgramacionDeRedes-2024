namespace LockExample1
{
    using System;
    using System.Threading;

    class DbContext
    {
        private readonly object lockObject = new object();

        public void SaveDataToDatabase(string data)
        {
            lock (lockObject)
            {
                Console.WriteLine("Guardando datos: " + data);

                if (data.Contains("error"))
                    throw new Exception("Error al guardar datos");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var db = new DbContext();

            var t1 = new Thread(() => Ejecutar(db, "Dato1"));
            var t2 = new Thread(() => Ejecutar(db, "error"));
            var t3 = new Thread(() => Ejecutar(db, "Dato2")); // habrá excepción, pero no se bloquea la zona crítica

            t1.Start(); t2.Start(); t3.Start();
            t1.Join(); t2.Join(); t3.Join();

            Console.WriteLine("Operación completada");
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
