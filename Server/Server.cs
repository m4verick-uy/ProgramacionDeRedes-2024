using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

namespace Servidor
{
    public class Server
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando app Servidor.....");
            
            //var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            
            TcpListener  tcpListener = new TcpListener(localEndPoint);
            tcpListener.Start();
            
            
            //socketServer.Bind(localEndPoint);  // Vinculamos el Socket y el endpoint
            //socketServer.Listen(10);  // Ponemos el socket en modo escucha
            bool salir = false;
            while (!salir)
            {

                //var socketClient = socketServer.Accept(); // BLOQUEANTE
                TcpClient  tcpClient = tcpListener.AcceptTcpClient(); // Tambien BLOQUEANTE
                
                Console.WriteLine("Acepte un cliente");

                var thread = new Thread(() => HandleClient(tcpClient));
                thread.Start();

            }
        }

        static void HandleClient(TcpClient tcpClient)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            while (true)
            {
                // Recepción del largo del mensaje
                byte[] dataLength = new byte[Protocol.WordLength];
                int totalReceived = 0;
                while (totalReceived < Protocol.WordLength)
                {
                    int received = networkStream.Read(dataLength,
                        totalReceived,
                        Protocol.WordLength - totalReceived);
                    if (received == 0)
                    {
                    // Cerrar la conexión
                    // Lanzar excepción o lo que sea necesario para terminar el thread
                        networkStream.Close();
                        Console.WriteLine("Closing connection...");
                        return;
                    }
                    totalReceived += received;
                }
                var length = BitConverter.ToInt32(dataLength, 0);
                // Recepción del mensaje
                byte[] data = new byte[length];
                totalReceived = 0;
                while (totalReceived < length)
                {
                    int received = networkStream.Read(data,
                        totalReceived,
                        length - totalReceived);
                    if (received == 0)
                    {
                    // Cerrar la conexión
                    // Lanzar excepción o lo que sea necesario para terminar el thread
                        networkStream.Close();
                        Console.WriteLine("Closing connection...");
                        return;
                    }
                    totalReceived += received;
                }
                string clientData = Encoding.UTF8.GetString(data);
                Console.WriteLine($"Client says {clientData}");
            }

        }
    }
}
