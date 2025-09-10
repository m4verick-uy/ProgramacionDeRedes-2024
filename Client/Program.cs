using System.Net;
using System.Net.Sockets;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear el socket
            var socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // creamos un endpont, una asociacion IP/puerto

            // endpoint cliente local 
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15000);

            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);

            // vincular el endpoint con el socket 
            socketClient.Bind(localEndpoint);

            // cliente se conecta al servidor
            socketClient.Connect(remoteEndpoint);



            Console.WriteLine("Connected to server!!!!");
            Console.ReadLine();
            Console.WriteLine("Will Close Connection...");
            socketClient.Shutdown(SocketShutdown.Both); 
            socketClient.Close(); 
        }
    }
}
