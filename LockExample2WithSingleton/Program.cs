// DemoA_Singleton_NoLock/Program.cs
namespace DemoA_Singleton_NoLock
{
    using System;
    using System.Threading;

    sealed class DbContext
    {
        // Singleton
        private static readonly object _instanceLock = new object();
        private static DbContext? _instance;
        public static DbContext Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null) _instance = new DbContext();
                    return _instance;
                }
            }
        }

        private DbContext() { }

        // ESTADO COMPARTIDO MUTABLE
        private int _totalWrites; // cambia con cada guardado

        public void SaveDataToDatabase(string data)
        {
            Console.WriteLine("Guardando datos: " + data);
            // Riesgo: ++ no es atómico a nivel lógico (read-modify-write)
            _totalWrites++;
        }

        public int TotalWrites => _totalWrites; // lectura sin protección (también riesgosa)
    }

    class Program
    {
        static void Main()
        {
            var db = DbContext.Instance;

            var t1 = new Thread(() => Ejecutar(db, "Dato1"));
            var t2 = new Thread(() => Ejecutar(db, "Dato2"));
            var t3 = new Thread(() => Ejecutar(db, "Dato3"));

            t1.Start(); t2.Start(); t3.Start();
            t1.Join(); t2.Join(); t3.Join();

            Console.WriteLine($"TotalWrites reportado: {db.TotalWrites} (podría no coincidir con el esperado)");
        }

        static void Ejecutar(DbContext db, string dato)
        {
            for (int i = 0; i < 10000; i++)
                db.SaveDataToDatabase(dato);
        }
    }
}
