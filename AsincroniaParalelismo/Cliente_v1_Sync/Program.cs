using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        string nombre = args.Length > 0 ? args[0] : "pedido1";
        int demora = args.Length > 1 ? int.Parse(args[1]) : 3000;

        Stopwatch reloj = Stopwatch.StartNew();
        string respuesta = Consultar(nombre, demora);
        Console.WriteLine("{0} ({1} ms)", respuesta, reloj.ElapsedMilliseconds);
    }

    static string Consultar(string nombre, int demora)
    {
        IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        IPEndPoint remoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

        using TcpClient cliente = new TcpClient(local);
        cliente.Connect(remoto);
        NetworkStream stream = cliente.GetStream();

        Protocolo.Enviar(stream, nombre + "#" + demora);
        return Protocolo.Recibir(stream);
    }
}
