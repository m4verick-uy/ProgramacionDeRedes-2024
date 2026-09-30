using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ejemplo3;

class Program
{
    // private static string RunLongTask(string taskName)
    // {
    //     Task.Delay(5000);   // crea la tarea... y la ignora
    //     return taskName + " Completed!";
    // }
    
    private static async Task<string> RunLongTaskAsync(string taskName)
    {
        await Task.Delay(5000);   // ahora sí espera
        return taskName + " Completed!";
    }
    
    static  async /*void*/ Task Main(string[] args)
    {
        Stopwatch reloj = Stopwatch.StartNew();
        for (int i = 0; i <= 3; i++)
        {
            string resultado =  await RunLongTaskAsync("Task " + i);
            Console.WriteLine("{0} ({1} ms)", resultado, reloj.ElapsedMilliseconds);
        }
    }
}