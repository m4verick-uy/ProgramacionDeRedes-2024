using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Cliente
{
    public class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando app Cliente...!");

            var socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 toma el primero disponible
            socketCliente.Bind(localEndPoint);

            // El endpoint del servidor
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);

            socketCliente.Connect(remoteEndPoint); // Nos conectamos al servidor

            Console.WriteLine("Me conecté al servidor!!!! ");

            Console.WriteLine("Escriba un mensaje y presione enter");
            while (true)
            {
                string mensaje = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(mensaje);
                socketCliente.Send(data);
            }

            Console.WriteLine("Cerrando la conexion");

            socketCliente.Shutdown(SocketShutdown.Both);
            socketCliente.Close();

        }
    }
}
