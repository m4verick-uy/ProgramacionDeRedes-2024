using System;
using System.Threading;

class DbContext
{
    private readonly object lockObject = new object();

    public void SaveDataToDatabase(string data)
    {
        lock (lockObject)
        {
            // Simulamos la operación de guardar
            Console.WriteLine("Guardando datos: " + data);

            // La excepción NACE acá, en lo más profundo de la pila del hilo.
            // Desde acá empieza a subir buscando un catch en la misma pila.
            if (data.Contains("error"))
            {
                throw new Exception("Error al guardar datos");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        DbContext dbContext = new DbContext();

        Thread t1 = new Thread(() =>
        {
            try
            {
                dbContext.SaveDataToDatabase("Dato1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción capturada en t1: {ex.Message}");
            }
        });

        Thread t2 = new Thread(() =>
        {
            try
            {
                dbContext.SaveDataToDatabase("Dato2");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción capturada en t2: {ex.Message}");
            }
        });

        Thread t3 = new Thread(() =>
        {
            try
            {
                // La lambda llama a SaveDataToDatabase, que lanzará la excepción.
                // Esta sube desde SaveDataToDatabase hasta este catch, todo dentro de t3.
                dbContext.SaveDataToDatabase("error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción capturada en t3: {ex.Message}");
            }
        });

        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine("Continúa hilo principal, operación completada");
    }
}
