using System.Net.Sockets;

namespace CommonLibrary;

// Envio y recepcion de datos sobre un Socket, garantizando que se transfieran
// TODOS los bytes. Send y Receive pueden mover menos bytes de los pedidos, asi
// que iteramos hasta completar. (Ver la Guia 2 de Sockets del curso)

public class NetworkDataHelper
{
    private readonly Socket _socket;

    public NetworkDataHelper(Socket socket)
    {
        _socket = socket;
    }

    // Envia todos los bytes de 'datos', reintentando hasta terminar.
    public void EnviarTodos(byte[] datos)
    {
        int offset = 0;
        int size = datos.Length;

        while (offset < size)
        {
            int enviados = _socket.Send(datos, offset, size - offset, SocketFlags.None);
            if (enviados == 0)
                throw new SocketException(); // el otro extremo cerro la conexion
            offset += enviados;
        }
    }

    // Recibe exactamente 'cantidad' bytes, reintentando hasta completarlos.
    public byte[] RecibirTodos(int cantidad)
    {
        byte[] datos = new byte[cantidad];
        int offset = 0;

        while (offset < cantidad)
        {
            int recibidos = _socket.Receive(datos, offset, cantidad - offset, SocketFlags.None);
            if (recibidos == 0)
                throw new SocketException(); // Receive devolvio 0: el otro extremo cerro
            offset += recibidos;
        }

        return datos;
    }
}
