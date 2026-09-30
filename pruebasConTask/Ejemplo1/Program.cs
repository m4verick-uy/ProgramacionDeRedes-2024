namespace pruebasConTask;

using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var tarea = new Task(() =>
        {
            Thread.Sleep(1000);
            Console.WriteLine("La tarea interna");
        });

        tarea.Start();
        //await tarea;
        Console.WriteLine("La tarea ha terminado");
        Console.ReadLine();
    }
}