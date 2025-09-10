using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server - Waiting for clients connections ......");
            
            // Creo la clase Sockets
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000); // De 0  a 65535 pero hay algunos ptos reservados, mejor > 20000

            //Vinculamos el socket con el endpoint
            socketServer.Bind(localEndpoint);

            socketServer.Listen(1); 

            var socketClient = socketServer.Accept(); // OJO CON ESTA OPEARACION ES BLOQUEANTE !!!!


            Console.WriteLine("Solo se imprime una vez que el servidor acepto una conexion"); 
            Console.ReadLine();
        }
    }
}
