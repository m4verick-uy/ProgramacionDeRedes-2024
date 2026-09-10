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
            try
            {
                // ===== CASO 1 - SIN CONTROL DE LARGO (buffer fijo) =====
                //byte[] data = new byte[256];
                //int byteRecibidos = socketClient.Receive(data);

                // ===== CASO 2 - LARGO COMO TEXTO ("0007") =====
                // byte[] header = new byte[4];
                // socketClient.Receive(header);
                // int largo = int.Parse(Encoding.UTF8.GetString(header));
                //
                // byte[] data = new byte[largo];
                // int byteRecibidos = socketClient.Receive(data);

                // ===== CASO 3 - LARGO COMO ENTERO (Int32 binario) =====
                byte[] header = new byte[4];
                socketClient.Receive(header);
                int largo = BitConverter.ToInt32(header, 0);
                
                byte[] data = new byte[largo];
                int byteRecibidos = socketClient.Receive(data);

                // ----- común a CASO 2 y 3 -----
                if (byteRecibidos == 0)
                {
                    ClientConnected = false; // el cliente cerró la conexión
                }
                else
                {
                    string mensaje = Encoding.UTF8.GetString(data);
                    Console.WriteLine("El cliente dice: " + mensaje);
                }
            }
            catch (SocketException)
            {
                ClientConnected = false; // corte abrupto
            }
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