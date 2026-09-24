using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
        TcpListener listener = new TcpListener(local);
        listener.Start();
        Console.WriteLine("Servidor escuchando en {0}", local);

        while (true)
        {
            TcpClient cliente = listener.AcceptTcpClient();
            AtenderCliente(cliente);   // hasta que no termina, no aceptamos otro
        }
    }

    static void AtenderCliente(TcpClient cliente)
    {
        NetworkStream stream = cliente.GetStream();
        string pedido = Protocolo.Recibir(stream);        // "nombre#demoraMs"
        string[] partes = pedido.Split('#');
        string nombre = partes[0];
        int demora = int.Parse(partes[1]);

        Console.WriteLine("[{0:HH:mm:ss}] Atendiendo {1} ({2} ms) - hilo {3}",
            DateTime.Now, nombre, demora, Environment.CurrentManagedThreadId);

        Thread.Sleep(demora);      // simulamos el trabajo (BD, API externa, etc.)

        Protocolo.Enviar(stream, "Listo " + nombre);
        cliente.Close();
    }
}
