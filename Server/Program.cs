using Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting Server Application..");
        var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);

        socketServer.Bind(localEndpoint);
        socketServer.Listen(3); // Nuestro Socket pasa a estar en modo escucha
        Console.WriteLine("Waiting for clients...");
        while (true)
        {
            Socket clientSocket = socketServer.Accept(); // El accept es bloqueante, espera hasta que llega una nueva conexión
            Console.WriteLine("Client connected");
            new Thread(() => HandleClient(clientSocket)).Start(); // Lanzamos un nuevo hilo para manejar al nuevo cliente
        }
    }

    static void HandleClient(Socket clientSocket)
    {
        bool clientIsConnected = true;
        NetworkDataHelper networkDataHelper = new NetworkDataHelper(clientSocket);
        const int largoDataLength = 4; // Pasar a una clase con constantes del protocolo
        while (clientIsConnected)
        {
            try
            {
                byte[] dataLength = networkDataHelper.Receive(largoDataLength); // Recibo la parte fija de los datos
                byte[] data = networkDataHelper.Receive(BitConverter.ToInt32(dataLength)); // Recibo los datos (parte variable)
                string cmd = Encoding.UTF8.GetString(data);

                switch (cmd)
                {
                    case "1":
                        HandleRegister(networkDataHelper);
                        break;
                    case "2":
                        HandleLogin(networkDataHelper);
                        break;
                    default:
                        clientIsConnected = false;
                        break;
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Client disconnected");
                clientIsConnected = false;
            }
        }
    }

    static void HandleRegister(NetworkDataHelper networkDataHelper)
    {
        string response = "Register logic not implemented yet.";
        SendMessage(networkDataHelper, response);
    }

    static void HandleLogin(NetworkDataHelper networkDataHelper)
    {
        SendMessage(networkDataHelper, "OK");

        // Receive username from client
        string username = ReceiveMessage(networkDataHelper);

        // Receive password from client
        string password = ReceiveMessage(networkDataHelper);

        // Validate user
        if (UserManager.ValidateUser(username, password))
        {
            SendMessage(networkDataHelper, "Login successful.");
        }
        else
        {
            SendMessage(networkDataHelper, "Error in username or password.");
        }
    }

    private static void SendMessage(NetworkDataHelper networkDataHelper, string message)
    {
        networkDataHelper.Send(message);
    }
    
    private static string ReceiveMessage(NetworkDataHelper networkDataHelper)
    {
        return networkDataHelper.Receive();
    }
}