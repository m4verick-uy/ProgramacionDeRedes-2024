namespace threadStartExample;

using System.Threading;

class Program
{

    // Ejemplo 1 — ThreadStart explícito
    // Sin parámetros: método pasado al delegado ThreadStart de forma explícita.
    // ThreadStart -> se arranca con Start().

    static void Main(string[] args)
    {
        Thread thread = new Thread(new ThreadStart(DoWork));
        thread.Start();
        thread.Join();
        Console.WriteLine("Continúa el Main y su hilo principal");
    }
    static void DoWork()
    {
        Console.WriteLine("Hilo corriendo con delegado ThreadStart explícito");
        for (int i = 0; i <= 10; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(500);
        }
    }


    // Ejemplo 2 — Función directa
    // Sin parámetros: método pasado directo, sin nombrar el delegado.
    // Tras bambalinas C# igual usa ThreadStart -> se arranca con Start().


    /*static void Main (string[] args)
    {
        Thread thread = new Thread(DoWork);
        thread.Start();
        thread.Join();
        Console.WriteLine("Continúa el Main y su hilo principal");
    }

    static void DoWork()
    {
        Console.WriteLine("Hilo corriendo con función directa (ThreadStart implícito)");
        for (int i = 0; i <= 10; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(500);
        }
    }*/


    // Ejemplo 3 — Lambda anónima
    // Sin parámetros: lambda anónima sin argumentos.
    // Tras bambalinas es ThreadStart -> se arranca con Start().

    /*static void Main(string[] args)
    {
        Thread thread = new Thread(() =>
        {
            Console.WriteLine("Hilo corriendo con lambda anónima sin parámetros (ThreadStart)");
            for (int i = 0; i <= 10; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(500);
            }
        });
        thread.Start();
        thread.Join();
        Console.WriteLine("Continúa el Main y su hilo principal");
    }*/


    // Ejemplo 4 — Lambda con parámetro
    // Con parámetro: lambda que recibe un object.
    // Tras bambalinas es ParameterizedThreadStart -> se arranca con Start(10).

    // static void Main(string[] args)
    // {
    //     Thread thread = new Thread((object data) =>
    //     {
    //         Console.WriteLine("Hilo corriendo con lambda y parámetro (ParameterizedThreadStart)");
    //         for (int i = 0; i <= (int)data; i++)
    //         {
    //             Console.WriteLine(i);
    //             Thread.Sleep(500);
    //         }
    //     });
    //     thread.Start(10);
    //     thread.Join();
    //     Console.WriteLine("Continúa el Main y su hilo principal");
    // }


    // Ejemplo 5 — ParameterizedThreadStart explícito
    // Con parámetro: método pasado al delegado ParameterizedThreadStart de forma explícita.
    // El método debe tomar un object -> se arranca con Start(15).

    // static void Main(string[] args)
    // {
    //     var thread = new Thread(new ParameterizedThreadStart(DoWork));
    //     thread.Start(15);
    //     thread.Join();
    //     Console.WriteLine("Continúa el Main y su hilo principal");
    // }
    // // Presen atención acá, al método que le pasamos al delegado tiene que tomar un Object
    // // por eso hay que castearlo adentro con (int)data
    // static void DoWork(Object data)
    // {
    //     Console.WriteLine("Hilo corriendo con ParameterizedThreadStart explícito");
    //     for (var i = 0; i <= (int)data; i++)
    //     {
    //         Console.WriteLine(i);
    //         Thread.Sleep(500);
    //     }
    // }
}
