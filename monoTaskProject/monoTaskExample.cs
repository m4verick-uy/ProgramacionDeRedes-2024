namespace Threads;

using System;

class monoTaskExample
{
    static void Main(string[] args)
    {
        PrintNumbers();
        PrintLetters();
    }
    static void PrintNumbers()
    {
        for (int i = 0; i <= 5; i++)
        {
            Console.WriteLine(i);
            System.Threading.Thread.Sleep(500);
        }
    }

    static void PrintLetters()
    {
        for (char c = 'A'; c <= 'E'; c++)
        {
            Console.WriteLine(c);
            System.Threading.Thread.Sleep(500);
        }
    }
}