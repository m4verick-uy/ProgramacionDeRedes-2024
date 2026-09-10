namespace newClient;

using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    // ===== Envío garantizado (mismo código que el servidor) =====
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

    // ===== Recepción garantizada (mismo código que el servidor) =====
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

    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando aplicación cliente !!");

        var socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        socketClient.Bind(localEndPoint);
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
        socketClient.Connect(remoteEndPoint);

        Console.WriteLine("Conectado al servidor !!!");

        // Recibo el saludo del servidor con el protocolo completo
        byte[] header = RecibirTodo(socketClient, 4);
        int largo = BitConverter.ToInt32(header, 0);
        byte[] cuerpo = RecibirTodo(socketClient, largo);
        Console.WriteLine("El servidor dice: " + Encoding.UTF8.GetString(cuerpo));

        Console.WriteLine("Escriba un mensaje para enviar: ");

        bool clienteEstaCorriendo = true;
        while (clienteEstaCorriendo)
        {
            string mensaje = Console.ReadLine();
            if (mensaje == "exit")
            {
                clienteEstaCorriendo = false;
            }
            else
            {
                byte[] mensajeByts = Encoding.UTF8.GetBytes(mensaje);
                EnviarTodo(socketClient, BitConverter.GetBytes(mensajeByts.Length));
                EnviarTodo(socketClient, mensajeByts);
            }
        }

        socketClient.Shutdown(SocketShutdown.Both);
        socketClient.Close();
    }
}