namespace monitorBadExample1;

//Ejemplo sin try-finally (Incorrecto)

using System;
using System.Threading;

class DbContext
{
    private readonly object lockObject = new object();

    public void SaveDataToDatabase(string data)
    {
        Monitor.Enter(lockObject);
        // Simulo operación de guardar 
        Console.WriteLine("Guardando datos: " + data);
        
        if (data.Contains("error"))
        {
            throw new Exception("Error al guardar datos");
        }
        Monitor.Exit(lockObject);
    }
}

class Program
{
    static void Main()
    {
        DbContext dbContext = new DbContext();

        Thread t1 = new Thread(() => dbContext.SaveDataToDatabase("Dato1"));
        Thread t2 = new Thread(() => dbContext.SaveDataToDatabase("Dato2"));
        Thread t3 = new Thread(() => dbContext.SaveDataToDatabase("error"));

        t1.Start();
        t2.Start();
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine("Operación completada");
    }
}