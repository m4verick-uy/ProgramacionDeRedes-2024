namespace Client;

using System.Net;
using System.Net.Sockets;
using System.Text;


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando aplicación cliente !!");
        
        // creamos el socket cliente
        var socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        // creamos el endpoint local
        var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        
        // vinculamos socket con su endpoint local
        socketClient.Bind(localEndPoint);
        
        // creo el endpoint remoto del servidor 
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
        
        // conectamos el cliente al servidor
        socketClient.Connect(remoteEndPoint);
        
        Console.WriteLine("Conectado al servidor !!!");
        
        Console.WriteLine("Escriba un mensaje para enviar: ");

        while (true)
        {
            string mensaje = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes(mensaje); // toma un mensaje de string y lo pasa a bytes
            socketClient.Send(data);
        }
        
        // Configuro la desconexión segura
        socketClient.Shutdown(SocketShutdown.Both);
        socketClient.Close();
        
    }
}