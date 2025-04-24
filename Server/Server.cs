using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Servidor
{
    public class Server
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando app Servidor.....");

            //creo el socket
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            // creo el endpoint con IP y puerto  (Estos son conocidos)
            
            // lleva any para que pueda tomar cualquier IP asignada en este caso por la inyeccion del DNS de docker
            var localEndPoint = new IPEndPoint(IPAddress.Any, 10000);
            socketServer.Bind(localEndPoint);  // Vinculamos el Socket y el endpoint

            socketServer.Listen(10);  // Ponemos el socket en modo escucha
            bool salir = false;
            while (!salir)
            {

                var socketClient = socketServer.Accept(); // BLOQUEANTE
                Console.WriteLine("Acepte un cliente");

                new Thread(() => HandleClient(socketClient)).Start();
                // Lanzo un hilo por cada cliente
            }
            //Console.ReadLine();
        }

        static void HandleClient(Socket socketCliente)
        {
            Console.WriteLine("Atendiendo a un cliente");
            bool isConected = true;
            while (isConected)
            {
                byte[] data = new byte[1024];
                socketCliente.Receive(data);
                string mensaje = Encoding.UTF8.GetString(data);
                Console.WriteLine(mensaje);
            }
            Console.WriteLine("Cliente desconectado");

        }
    }
}
