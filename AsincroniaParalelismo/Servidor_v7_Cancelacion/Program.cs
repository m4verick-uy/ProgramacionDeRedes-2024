using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
        TcpListener listener = new TcpListener(local);
        listener.Start();
        Console.WriteLine("Servidor escuchando en {0}. Enter para detener.",
            local);

        // Una tarea que espera el Enter y pide la cancelación
        Task esperaEnter = Task.Run(() =>
        {
            Console.ReadLine();
            cts.Cancel();
        });

        List<Task> atendiendo = new List<Task>();
        try
        {
            while (true)
            {
                TcpClient cliente =
                    await listener.AcceptTcpClientAsync(cts.Token);
                atendiendo.RemoveAll(t => t.IsCompleted);
                atendiendo.Add(AtenderClienteAsync(cliente));
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Deteniendo: no aceptamos más clientes...");
        }
        finally
        {
            listener.Stop();
        }

        await Task.WhenAll(atendiendo);   // los que están en curso terminan
        Console.WriteLine("Servidor detenido.");
    }

    static async Task AtenderClienteAsync(TcpClient cliente)
    {
        try
        {
            NetworkStream stream = cliente.GetStream();
            string pedido = await Protocolo.RecibirAsync(stream);
            string[] partes = pedido.Split('#');
            string nombre = partes[0];
            int demora = int.Parse(partes[1]);

            Console.WriteLine("[{0:HH:mm:ss}] Atendiendo {1} ({2} ms)",
                DateTime.Now, nombre, demora);
            await Task.Delay(demora);
            await Protocolo.EnviarAsync(stream, "Listo " + nombre);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error atendiendo cliente: {0}", ex.Message);
        }
        finally
        {
            cliente.Close();
        }
    }
}
