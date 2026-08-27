
namespace SingletonWithResourceLock
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

        // ESTADO COMPARTIDO MUTABLE protegido
        private readonly object zonaCritica = new object();
        private int _totalWrites;

        public void SaveDataToDatabase(string data)
        {
          //  Console.WriteLine("Guardando datos: " + data);

            // comentar y ver resultado
            //lock (zonaCritica)
            {
                _totalWrites++; // ahora la actualización es excluyente
            }
        }
        public int TotalWrites
        {
            get
            {
                lock (zonaCritica)
                {
                    return _totalWrites; // lectura consistente
                }
            }
        }
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

            Console.WriteLine($"TotalWrites reportado: {db.TotalWrites} (debería ser 30000)");
        }

        static void Ejecutar(DbContext db, string dato)
        {
            for (int i = 0; i < 10000; i++)
                db.SaveDataToDatabase(dato);
        }
    }
}
