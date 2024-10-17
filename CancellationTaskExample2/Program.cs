namespace CancellationTaskExample2;

class Program
{
    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        
        var contador = contadorTask(token);

        Console.ReadKey();
        cts.Cancel();

        try
        {
            await contador;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Tarea cancelada");
        }
        finally
        {
            cts.Dispose();
        }
        
    }
    
    static async Task contadorTask(CancellationToken token)
    {

        for (int i = 0; i < 10; i++)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("¡Contador cancelado! 🚨");
                throw new TaskCanceledException();    
            }
            Console.WriteLine(i);
            await Task.Delay(2000);
        }

        Console.WriteLine("¡Contador finalizado! 🎉");
    }
    
}