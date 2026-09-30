using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        // ---- Opción 1: la tarea termina bien (activa por defecto) ----
        // Task<int> task = Task.Run(() =>
        // {
        //     return 10;
        // });

        // ---- Opción 2: la tarea falla ----
        // Func<int> intermedia porque, si el lambda solo tira una excepción,
        // el compilador no puede inferir que "debería" devolver int.
        //Func<int> metodoQueFalla = () => throw new Exception("falló");
        //Task<int> task = Task.Run(metodoQueFalla);

        // ---- Opción 3: la tarea se cancela ----
        CancellationTokenSource cts = new CancellationTokenSource();
        cts.Cancel();   // cancelamos ANTES de que arranque a correr
        Task<int> task = Task.Run(() =>
        {
            return 10;
        }, cts.Token);

        task.ContinueWith((i) =>
        {
            Console.WriteLine("TasK Canceled");
        }, TaskContinuationOptions.OnlyOnCanceled);

        task.ContinueWith((i) =>
        {
            Console.WriteLine("Task Faulted");
        }, TaskContinuationOptions.OnlyOnFaulted);

        var completedTask = task.ContinueWith((i) =>
        {
            Console.WriteLine("Task Completed");
        }, TaskContinuationOptions.OnlyOnRanToCompletion);

        try
        {
            completedTask.Wait();
        }
        catch (AggregateException)
        {
            // Si "task" falló o se canceló, esta continuación nunca corre:
            // ella misma queda en estado Canceled, y Wait() tira acá.
        }

        Console.ReadKey();
    }
}