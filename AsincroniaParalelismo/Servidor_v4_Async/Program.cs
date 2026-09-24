using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
        TcpListener listener = new TcpListener(local);
        listener.Start();
        Console.WriteLine("Servidor escuchando en {0}", local);

        while (true)
        {
            TcpClient cliente = await listener.AcceptTcpClientAsync();
            _ = AtenderClienteAsync(cliente);   // no esperamos
        }
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

            Console.WriteLine("[{0:HH:mm:ss}] Atendiendo {1} ({2} ms) - hilo {3}",
                DateTime.Now, nombre, demora, Environment.CurrentManagedThreadId);

            await Task.Delay(demora);

            Console.WriteLine("[{0:HH:mm:ss}] Terminando {1} - hilo {2}",
                DateTime.Now, nombre, Environment.CurrentManagedThreadId);

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
