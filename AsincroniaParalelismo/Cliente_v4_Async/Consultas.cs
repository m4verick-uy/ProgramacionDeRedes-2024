using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

static class Consultas
{
    public static async Task<string> ConsultarAsync(string nombre, int demora)
    {
        IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        IPEndPoint remoto = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

        using TcpClient cliente = new TcpClient(local);
        await cliente.ConnectAsync(remoto);
        NetworkStream stream = cliente.GetStream();

        await Protocolo.EnviarAsync(stream, nombre + "#" + demora);
        return await Protocolo.RecibirAsync(stream);
    }
}
