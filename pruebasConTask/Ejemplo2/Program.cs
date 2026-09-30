using System;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        // ---- Opción A: crear la tarea y arrancarla aparte ----
        //Task t1 = new Task(PrintInfo);
        //t1.Start();

        // ---- Opción B: Task.Run (ya arranca sola, no hace falta Start) ----
         Task t1 = Task.Run(() => { PrintInfo(); });
         
        Console.WriteLine("Main Thread Completed");
        Console.ReadLine();
    }

    static void PrintInfo()
    {
        for (int i = 1; i <= 4; i++)
        {
            Console.WriteLine("i value: {0}", i);
            Task.Delay(500);
        }
        Console.WriteLine("Child Thread Completed");
    }
}