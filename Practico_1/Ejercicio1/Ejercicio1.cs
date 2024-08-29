namespace Practico1;

using System.Threading;


// Solución usando Join()

/*
class Ejercicio1
{
    static void Main(string[] args)
    {
        //Creo los hilos
        Thread t1 = new Thread(PrintX);
        Thread t2 = new Thread(PrintY);
        
        //Lanzo los hilos
        t1.Start();
        t1.Join();
        
        t2.Start();
        t2.Join();
    }

    static void PrintX()
    {
        for (int i = 1; i <=20; i++)
        {
            Console.Write('X');
            Thread.Sleep(100);
        }
    }

    static void PrintY()
    {
        for (int i = 1; i <= 20; i++)
        {
            Console.Write('Y');
            Thread.Sleep(100);
        }
    }
}
*/

// Solucion usando AutoResetEvent para sincronizar los threads

class Ejericio1
{
    static AutoResetEvent autoEvent = new AutoResetEvent(false);
    
    static void Main(string[] args)
    {
        //Creo los hilos
        Thread t1 = new Thread(PrintX);
        Thread t2 = new Thread(PrintY);
        
        //Lanzo los hilos
        t1.Start();
        t2.Start();
    }

    static void PrintX()
    {
        for (int i = 1; i <=20; i++)
        {
            Console.Write('X');
            Thread.Sleep(100);
        }
        autoEvent.Set(); //Notifico que PrintX terminó
    }

    static void PrintY()
    {
        autoEvent.WaitOne(); //Espero hasta que PrintX termine
        for (int i = 1; i <= 20; i++)
        {
            Console.Write('Y');
            Thread.Sleep(100);
        }
    }
}