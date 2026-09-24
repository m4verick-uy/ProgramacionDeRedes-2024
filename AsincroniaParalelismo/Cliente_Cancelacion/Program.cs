using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 1) Cancelación automática por tiempo
        using CancellationTokenSource conTimeout =
            new CancellationTokenSource(TimeSpan.FromSeconds(2));
        try
        {
            string respuesta =
                await Consultas.ConsultarAsync("lenta", 5000, conTimeout.Token);
            Console.WriteLine(respuesta);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Cancelada: pasaron más de 2 segundos");
        }

        // 2) Cancelación manual
        using CancellationTokenSource manual = new CancellationTokenSource();
        Task<string> larga =
            Consultas.ConsultarAsync("larga", 10000, manual.Token);

        await Task.Delay(1000);
        manual.Cancel();

        try
        {
            await larga;
        }
        catch (OperationCanceledException)
        {
            // La tarea quedó en estado Canceled
            Console.WriteLine("Estado de la tarea: {0}", larga.Status);
        }
    }
}
