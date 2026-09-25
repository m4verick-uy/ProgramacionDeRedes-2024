using System;
using System.Threading;

class Program
{
    static readonly object zonaCritica = new object();
    static int trabajosPendientes = 8;

    static void Main(string[] args)
    {
        Console.WriteLine("Mandamos 8 trabajos cortos al ThreadPool...");
        Console.WriteLine();

        for (int i = 1; i <= 8; i++)
        {
            int numero = i;   // copia local: cada trabajo usa su propio número
            ThreadPool.QueueUserWorkItem((object estado) =>
            {
                Console.WriteLine("Trabajo {0} lo atiende el hilo {1}",
                    numero, Environment.CurrentManagedThreadId);
                Thread.Sleep(500);   // simulamos algo de trabajo
                lock (zonaCritica)
                {
                    trabajosPendientes--;
                }
            });
        }

        while (trabajosPendientes > 0)
        {
            Thread.Sleep(50);   // esperamos a que terminen los 8
        }

        Console.WriteLine();
        Console.WriteLine("Los 8 trabajos terminaron.");
        Console.WriteLine("Se repiten números de hilo. No se creó un hilo por trabajo,");
        Console.WriteLine("Un grupo chico de hilos se fue turnando para atenderlos a todos.");
    }
}