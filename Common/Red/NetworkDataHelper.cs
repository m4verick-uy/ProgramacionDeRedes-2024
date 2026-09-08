using System.Net.Sockets;
using System.Text;
using Common.Protocolo;

namespace Common.Red;

/// <summary>
/// Envío y recepción de datos sobre un Socket, garantizando que se manden y
/// reciban TODOS los bytes (Send/Receive pueden transferir de a partes).
///
/// La lógica de Send/Receive con while + offset está tomada tal cual de la
/// "Guía Protocolo de Comunicación" del curso.
///
/// Encima de eso, agregamos EnviarMensaje / RecibirMensaje que arman y leen la
/// trama completa del protocolo:  CMD(2) + LARGO(4) + DATOS.
/// </summary>
public class NetworkDataHelper
{
    private readonly Socket _socket;

    public NetworkDataHelper(Socket socket)
    {
        _socket = socket;
    }

    // ---------------- Primitivas (de la guía) ----------------

    // Envía todos los bytes de 'data', reintentando hasta terminar.
    public void Send(byte[] data)
    {
        int offset = 0;
        int size = data.Length;

        while (offset < size) // sigo enviando hasta llegar al final
        {
            int sent = _socket.Send(data, offset, size - offset, SocketFlags.None);
            if (sent == 0) // el otro extremo cerró la conexión
            {
                throw new SocketException();
            }
            offset += sent; // la próxima iteración arranca donde terminó esta
        }
    }

    // Recibe exactamente 'length' bytes, reintentando hasta completarlos.
    public byte[] Receive(int length)
    {
        byte[] response = new byte[length];
        int offset = 0;

        while (offset < length) // sigo recibiendo hasta completar el buffer
        {
            int received = _socket.Receive(response, offset, length - offset, SocketFlags.None);
            if (received == 0) // el otro extremo cerró la conexión
            {
                throw new SocketException();
            }
            offset += received;
        }

        return response;
    }

    // ---------------- Trama del protocolo ----------------

    /// <summary>
    /// Envía un mensaje completo: CMD(2) + LARGO(4) + DATOS.
    /// </summary>
    public void EnviarMensaje(Comando comando, byte[] datos)
    {
        datos ??= Array.Empty<byte>();

        // CMD: número de comando a texto de 2 caracteres ("01", "27", "50"...).
        string cmdTexto = ((int)comando).ToString().PadLeft(ProtocoloConstantes.LargoCmd, '0');
        byte[] cmdBytes = Encoding.UTF8.GetBytes(cmdTexto);

        // LARGO: entero de 4 bytes con el tamaño de DATOS.
        byte[] largoBytes = BitConverter.GetBytes(datos.Length);

        Send(cmdBytes);
        Send(largoBytes);
        Send(datos);
    }

    // Sobrecarga cómoda: enviar directamente un Paquete ya armado.
    public void EnviarMensaje(Comando comando, Paquete paquete)
        => EnviarMensaje(comando, paquete.ObtenerBytes());

    /// <summary>
    /// Recibe un mensaje completo y devuelve el comando y sus datos.
    /// Lee primero la parte fija (CMD + LARGO) y luego la parte variable (DATOS).
    /// </summary>
    public (Comando comando, byte[] datos) RecibirMensaje()
    {
        // Parte fija: CMD.
        byte[] cmdBytes = Receive(ProtocoloConstantes.LargoCmd);
        int cmdNumero = int.Parse(Encoding.UTF8.GetString(cmdBytes));
        Comando comando = (Comando)cmdNumero;

        // Parte fija: LARGO.
        byte[] largoBytes = Receive(ProtocoloConstantes.LargoLargo);
        int largoDatos = BitConverter.ToInt32(largoBytes);

        // Parte variable: DATOS.
        byte[] datos = largoDatos > 0 ? Receive(largoDatos) : Array.Empty<byte>();

        return (comando, datos);
    }
}
