namespace newServer;

using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    // ===== Envío garantizado: insiste con offset/size hasta mandar todo =====
    static void EnviarTodo(Socket socket, byte[] data)
    {
        int offset = 0;
        int size = data.Length;
        while (offset < size)
        {
            int sent = socket.Send(data, offset, size - offset, SocketFlags.None);
            if (sent == 0)
            {
                throw new SocketException();
            }
            offset += sent;
        }
    }

    // ===== Recepción garantizada: insiste hasta juntar 'length' bytes =====
    static byte[] RecibirTodo(Socket socket, int length)
    {
        byte[] response = new byte[length];
        int offset = 0;
        while (offset < length)
        {
            int received = socket.Receive(response, offset, length - offset, SocketFlags.None);
            if (received == 0)
            {
                throw new SocketException();
            }
            offset += received;
        }
        return response;
    }

    static void HandleClient(Socket socketClient)
    {
        // Saludo de bienvenida con el protocolo completo (header Int32 + cuerpo)
        byte[] cuerpoSaludo = Encoding.UTF8.GetBytes("BIENVENIDO");
        EnviarTodo(socketClient, BitConverter.GetBytes(cuerpoSaludo.Length));
        EnviarTodo(socketClient, cuerpoSaludo);

        bool ClientConnected = true;
        while (ClientConnected)
        {
            try
            {
                byte[] header = RecibirTodo(socketClient, 4);
                int largo = BitConverter.ToInt32(header, 0);
                byte[] data = RecibirTodo(socketClient, largo);
                string mensaje = Encoding.UTF8.GetString(data);
                Console.WriteLine("El cliente dice: " + mensaje);
            }
            catch (SocketException)
            {
                ClientConnected = false;
            }
        }
        Console.WriteLine("Cliente desconectado");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando Servidor ....");

        var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
        socketServer.Bind(localEndpoint);
        socketServer.Listen(3);

        while (true)
        {
            var socketClient = socketServer.Accept();
            Console.WriteLine("Acepte un pedido de conexión");
            new Thread(() => HandleClient(socketClient)).Start();
        }
    }
}