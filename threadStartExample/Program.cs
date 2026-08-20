namespace threadStartExample;

using System.Threading;

class Program
{
    
    /*static void Main(string[] args)
    {
        // Ejemplo usando un delegado que representa el método que se ejecutará en un subproceso.
        Thread thread = new Thread(new ThreadStart(DoWork));
        thread.Start();
        thread.Join();
        Console.WriteLine("Continue main Thread...");
    }
    static void DoWork()
    {
        Console.WriteLine("Hilo corriendo con delegado ThreadStart");
        for (int i = 0; i <= 10; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(500);
        }
    }*/
    
    
    // #####################################################################
    
    
    // Ejempplo sin usar delegado ThreadStart 
    /*static void Main (string[] args)
    {
        Thread thread = new Thread(DoWork);
        thread.Start();
        thread.Join();
        Console.WriteLine("Continue main Thread...");    
    }
    
    static void DoWork()
    {
        Console.WriteLine("Thread is running without delegado ThreadStart");
        for (int i = 0; i <= 10; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(500);
        }
    }*/
    
    //#####################################################################
    
    // Ejemplo usando expresión lambda
    
    /*static void Main(string[] args)
    {
        Thread thread = new Thread(() =>
        {
            Console.WriteLine("Thread is running with delegado ThreadStart");
            for (int i = 0; i <= 10; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(500);
            }
        });
        thread.Start();
        thread.Join();
        Console.WriteLine("Continue main Thread...");
    }*/
    
    //#####################################################################
    
    // Ejemplo usando pametros en el método
    
    /*static void Main(string[] args)
    {
        Thread thread = new Thread((object data) =>
        {
            Console.WriteLine("Thread is running with delegado ThreadStart");
            for (int i = 0; i <= (int)data; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(500);
            }
        });
        thread.Start(10);
        thread.Join();
        Console.WriteLine("Continue main Thread...");
    }*/
    
    //#####################################################################
    
    // Ejemplo usando ParametricThreadStart

    static void Main(string[] args)
    {
        var thread = new Thread(new ParameterizedThreadStart(DoWork));
        thread.Start(10);
        thread.Join();
        Console.WriteLine("Continue main Thread...");
    }
    
    static void DoWork(Object data)
    {
        Console.WriteLine("Hilo corriendo con delegado pamametrizable ThreadStart");
        for (var i = 0; i <= (int)data; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(500);
        }
    }
}