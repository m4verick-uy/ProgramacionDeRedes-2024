namespace backgroundThreadsExample;

using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        //Creamos hilos de ejecución
        Thread t1 = new Thread(PrintNumbers);
        Thread t2 = new Thread(PrintLetters);
        
        //Ejecuto los threads en background
        t1.IsBackground = true;
        t2.IsBackground = true;
        
        //Inicio los hilos
        t1.Start();
        t2.Start();
        
        Console.WriteLine("El hilo principal termina y no espera a los hilos de background.");
    }
    
    static void PrintNumbers()
    {
        for(int i = 0; i<=5; i++)
        {
            System.Console.WriteLine(i);
            Thread.Sleep(500);
        }     
    }

    static void PrintLetters()
    {
        for (char l = 'A'; l <= 'E'; l++)
        {
            System.Console.WriteLine(l);
            Thread.Sleep(500);
        }
    }
}