using System;
using System.Threading;

class Panaderia
{
    private readonly object bandeja = new object();
    private bool hayPan = false;

    public void Hornear()  // el panadero
    {
        for (int i = 1; i <= 5; i++)
        {
            lock (bandeja)
            {
                Console.WriteLine($"Panadero: horneando pan {i}...");
                Thread.Sleep(500);
                hayPan = true;
                Console.WriteLine($"Panadero: pan {i} listo en la bandeja.");

                Monitor.Pulse(bandeja);  // avisa al cliente: "ya hay pan, despertate"
            }
            Thread.Sleep(200);
        }
    }

    public void Comprar()  // el cliente
    {
        for (int i = 1; i <= 5; i++)
        {
            lock (bandeja)
            {
                // Si no hay pan, espero. Wait suelta el lock mientras duermo.
                while (!hayPan)
                {
                    Console.WriteLine("Cliente: bandeja vacía, espero...");
                    Monitor.Wait(bandeja);  // duerme y suelta el lock hasta que lo despierten
                }

                hayPan = false;
                Console.WriteLine($"Cliente: retiré el pan {i}. ");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        var pan = new Panaderia();

        var panadero = new Thread(pan.Hornear);
        var cliente = new Thread(pan.Comprar);

        cliente.Start();   // arranca primero: va a encontrar la bandeja vacía y esperar
        panadero.Start();

        panadero.Join();
        cliente.Join();

        Console.WriteLine("Panadería cerrada.");
    }
}
