namespace multiTaskExample;

using System.Threading;

class multiTaskExample
{
    static void Main(string[] args)
    {
        // Create two threads
        Thread t1 = new Thread(PrintNumbers);
        Thread t2 = new Thread(PrintLetters);
        
        // Start the threads
        t1.Start();
        t2.Start();
        
        
        // Wait for the threads to finish
        //t1.Join();
        //t2.Join();
        
        Console.WriteLine("El hilo principal termina y no espera a los hilos de background.");
    }
    
    static void PrintNumbers()
    {
        for (int i = 0; i <= 10; i++)
        {
            Console.WriteLine(i);
            Thread.Sleep(500);
        }
    }

    static void PrintLetters()
    {
        for (char c = 'A'; c <= 'E'; c++)
        {
            Console.WriteLine(c);
            Thread.Sleep(500);
        }
    }
}