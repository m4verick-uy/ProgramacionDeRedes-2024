using System;
using System.Threading;

class DbContext
{
    private readonly object lockObject = new object();

    public void SaveDataToDatabase(string data)
    {
        Monitor.Enter(lockObject);
        try
        {
            // Simulamos la operación de guardar datos en la base de datos
            Console.WriteLine("Guardando datos: " + data);

            // Aquí podría ocurrir una excepción, por ejemplo, si hay un problema en la operación de escritura
            if (data.Contains("error"))
            {
                throw new Exception("Error al guardar datos");
            }
        }
        finally
        {
            Monitor.Exit(lockObject); // Asegura que el bloqueo se libere incluso si ocurre una excepción
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

        Console.WriteLine("Operación completada");
    }
}