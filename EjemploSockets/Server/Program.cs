namespace Server;

using System.Net;
using System.Net.Sockets;
using System.Text;


class Program
{

    static void HandleClient(Socket socketClient)
    {
        bool ClientConnected = true;

        while (ClientConnected)
        {
            byte[] data = new byte[256];
            socketClient.Receive(data);
            string message = Encoding.UTF8.GetString(data);
            Console.WriteLine("El cliente dice: " + message);
        }
        Console.WriteLine("Cliente desconectado");
    }
    
    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando Servidor ....");
        
        // crear el socket 
        var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        // creamos el endpoint 
        var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
        
        // vincular endpoint con el socket
        
        socketServer.Bind(localEndpoint);
        
        // configuramos el servidor en modo escucha
        socketServer.Listen(3);
        
        // configuramos que el servidor acepte conexiones



        while (true)
        {
            var socketClient = socketServer.Accept();  // Esta operacion es Bloqueante !!!!! CUIDADO
            Console.WriteLine("Acepte un pedido de conexiòn");
            
            new Thread(()=> HandleClient(socketClient)).Start();
        }
        
        
        
        Console.ReadLine();
        
        socketServer.Shutdown(SocketShutdown.Both);
        socketServer.Close();
    }
}