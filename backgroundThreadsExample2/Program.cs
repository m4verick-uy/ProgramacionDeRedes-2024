namespace backgroundThreadsExample;

using System.Threading;

class Program
{

    // Dos hilos haciendo el mismo tipo de trabajo (contar).
    // - El hilo FOREGROUND cuenta rápido y termina pronto.
    // - El hilo BACKGROUND cuenta lento y necesitaría más tiempo.

    static void Main(string[] args)
    {
        // Hilo foreground (por defecto): cuenta 5 veces, cada 300 ms.
        Thread primerPlano = new Thread(() => Contar("FOREGROUND", 5, 300));

        // Hilo background: cuenta 20 veces, cada 300 ms (necesita mucho más tiempo).
        Thread segundoPlano = new Thread(() => Contar("background", 20, 300));


        //Prueben comentar esta línea para que ambos hilos corran en primer plano.
        segundoPlano.IsBackground = true;

        primerPlano.Start();
        segundoPlano.Start();

        // El Main sigue vivo mientras haya algún hilo foreground trabajando.
    }

    static void Contar(string nombre, int hasta, int esperaMs)
    {
        for (int i = 1; i <= hasta; i++)
        {
            Console.WriteLine($"[{nombre}] {i}");
            Thread.Sleep(esperaMs);
        }
        Console.WriteLine($"[{nombre}] TERMINÉ de contar hasta {hasta}.");
    }
}
