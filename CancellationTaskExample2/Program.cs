namespace CancellationTaskExample2;

class Program
{
    static async Task Main(string[] args)
    {
        // Crear un CancellationTokenSource
        CancellationTokenSource cts = new CancellationTokenSource();
        
        // Obtengo un token de cancelación
        CancellationToken token = cts.Token;
        
        // Creo una tarea que se cancelará si el token de cancelación se activa
        var contador = contadorTask(token);
        
        // Leo cualquier tecla para cancelar la tarea
        Console.ReadKey();
        
        // Cancelo la tarea
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
            // Libero los recursos
            cts.Dispose();
        }
        
    }
    
    static async Task contadorTask(CancellationToken token)
    {

        for (int i = 0; i < 10; i++)
        {
            if (token.IsCancellationRequested) // Verifico si el token de cancelación se ha activado
            {
                Console.WriteLine("¡Contador cancelado! 🚨");
                // Lanzo una excepción para salir del bucle
                throw new TaskCanceledException();    
            }
            Console.WriteLine(i);
            await Task.Delay(2000);
        }

        Console.WriteLine("¡Contador finalizado! 🎉");
    }
}