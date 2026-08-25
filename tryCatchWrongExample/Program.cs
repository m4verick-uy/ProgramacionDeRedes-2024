using System.Threading;

class Program
{
    static void Main()
    {
        try
        {
            Thread t1 = new Thread(Procesar);
            t1.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocurrió una excepción");
        }
    }

    static void Procesar()
    {
        throw new Exception("Algo salió mal");
    }
}