namespace monitorBadExample1;

//Ejemplo sin try-finally (Incorrecto)

using System;
using System.Threading;

class DbContext
{
    private readonly object lockObject = new object();

    public void SaveDataToDatabase(string data)
    {
        Monitor.Enter(lockObject); // Se adquiere el bloqueo

        // Simulamos la operación de guardar datos en la base de datos
        Console.WriteLine("Guardando datos: " + data);

        // Aquí podría ocurrir una excepción, por ejemplo, si hay un problema en la operación de escritura
        if (data.Contains("error"))
        {
            throw new Exception("Error al guardar datos");
        }

        // Si ocurre una excepción, el siguiente código no se ejecuta, y el bloqueo no se libera
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
