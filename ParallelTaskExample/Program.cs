namespace ParallelTaskExample;

class Program
{
    static async Task Main(string[] args)
    {
        var numeros = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        Console.WriteLine("Procesando tareas en paralelo...");
        var ProcesandoTareas = ProcesandoTareasEnParalelo(numeros);
        await ProcesandoTareas;
    }
    
    static async Task ProcesandoTareasEnSerie(List<int> numeros)
    {
        foreach (var numero in numeros)
        {
             Console.WriteLine($"Procesando la tarea {numero}...en el hilo {Thread.CurrentThread.ManagedThreadId}");
             Task.Delay(1000).Wait();
        }
    }

    static async Task ProcesandoTareasEnParalelo(List<int> numeros)
    {
        Parallel.ForEach(numeros, numero =>
        {
            Console.WriteLine($"Procesando la tarea {numero}...en el hilo {Thread.CurrentThread.ManagedThreadId}");
            Task.Delay(2000).Wait();
        });
    }
}