using Common;
using System.Net;
using System.Net.Sockets;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting Client Application..");

        var socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        var remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);

        socketClient.Bind(localEndpoint);
        Console.WriteLine("Connecting to server...");
        socketClient.Connect(remoteEndpoint);
        Console.WriteLine("Connected to server!!!!");

        bool clientRunning = true;
        NetworkDataHelper networkDataHelper = new NetworkDataHelper(socketClient);

        while (clientRunning)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1 - Register");
            Console.WriteLine("2 - Login");
            Console.WriteLine("3 - Exit");
            Console.Write("Choose an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.WriteLine("Register selected.");
                    SendMessage(networkDataHelper, option);
                    string response1 = ReceiveMessage(networkDataHelper);
                    Console.WriteLine($"Server says: {response1}");
                    break;
                case "2":
                    Console.WriteLine("Login selected.");
                    SendMessage(networkDataHelper, option);
                    string response2 = ReceiveMessage(networkDataHelper);

                    if (response2.Equals("OK"))
                    {
                        Console.WriteLine("Enter username: ");
                        string username = Console.ReadLine();
                        SendMessage(networkDataHelper, username);
                        Console.WriteLine("Enter password: ");
                        string password = Console.ReadLine();
                        SendMessage(networkDataHelper, password);

                        // Receive final response from server after sending username and password
                        string finalResponse = ReceiveMessage(networkDataHelper);
                        Console.WriteLine($"Server says: {finalResponse}");
                    }
                    else
                    {
                        Console.WriteLine($"Server says: {response2}");
                    }
                    break;
                case "3":
                    clientRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Will Close Connection...");
        socketClient.Shutdown(SocketShutdown.Both);
        socketClient.Close();
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