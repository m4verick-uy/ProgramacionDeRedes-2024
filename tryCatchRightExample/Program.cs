using System.Threading;

class Program
{
    static void Main()
    {
        Thread t1 = new Thread(Procesar);

        t1.Start();
        t1.Join();

        Console.WriteLine("Main termina normalmente.");
    }

    static void Procesar()
    {
        try
        {
            Console.WriteLine("El thread empieza a trabajar...");

            throw new Exception("Algo salió mal");

            Console.WriteLine("Esto nunca se ejecuta.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error controlado dentro del thread: {ex.Message}");
        }

        Console.WriteLine("El thread continúa.");
    }
}