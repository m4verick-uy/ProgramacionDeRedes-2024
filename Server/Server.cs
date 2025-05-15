using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

Console.WriteLine("Iniciando app Servidor.....");

string ipStr = ConfigurationManager.AppSettings["ServerIP"] ?? "0.0.0.0";
string portStr = ConfigurationManager.AppSettings["ServerPort"] ?? "10000";

var ip = IPAddress.Parse(ipStr);
var port = int.Parse(portStr);

var socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socketServidor.Bind(new IPEndPoint(ip, port));
socketServidor.Listen(10);

Console.WriteLine($"Servidor escuchando en {ip}:{port}");

while (true)
{
    var socketCliente = await socketServidor.AcceptAsync();
    _ = Task.Run(async () =>
    {
        Console.WriteLine("Cliente conectado");
        var buffer = new byte[1024];
        int cantidad = await socketCliente.ReceiveAsync(buffer, SocketFlags.None);
        var mensaje = Encoding.UTF8.GetString(buffer, 0, cantidad);
        Console.WriteLine($"Recibido: {mensaje}");

        socketCliente.Shutdown(SocketShutdown.Both);
        socketCliente.Close();
    });
}