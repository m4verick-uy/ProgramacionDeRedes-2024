using System.Net;
using System.Net.Sockets;
using System.Text;
using Common;

namespace Cliente
{
    public class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando app Cliente...!");

            //var socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Puerto 0 toma el primero disponible
            
            var tcpClient = new TcpClient(localEndPoint);
            
            //socketCliente.Bind(localEndPoint);
            
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10000);
            
            //socketCliente.Connect(remoteEndPoint); // Nos conectamos al servidor
            tcpClient.Connect(remoteEndPoint);
            
            Console.WriteLine("Me conecté al servidor!!!! ");
            
            
            // acá la mayor diferencia
            
            NetworkStream networkStream = tcpClient.GetStream();
            var option = "";
            var exit = false;
            while (!exit)
            {
                Console.WriteLine("Escriba un mensaje y presione enter. exit para salir !!");
                option = Console.ReadLine();
                if (option == "exit")
                {
                    exit = true;
                }
                else
                {
                    // obtengo datos
                    byte[] data = Encoding.UTF8.GetBytes(option);
                    // obtengo largo
                    byte[] dataLength = BitConverter.GetBytes(data.Length);
                    // enviamos largo del mensaje 
                    networkStream.Write(dataLength, 0, Protocol.WordLength);
                    // enviamos el mensaje 
                    networkStream.Write(data, 0, data.Length);
                }

            }
            networkStream.Close();
            
            Console.WriteLine("Cerrando la conexion");
            //socketCliente.Shutdown(SocketShutdown.Both);
            //socketCliente.Close();

        }
    }
}
