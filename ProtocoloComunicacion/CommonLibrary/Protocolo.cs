using System.Net.Sockets;
using System.Text;

namespace CommonLibrary;

//  Armamos y leenos la trama del protocolo:  LARGO(4) + DATOS(variable).
//  Todo viaja como texto (protocolo orientado a caracteres).
//  Usamos el NetworkDataHelper para asegurar que se envien/reciban todos los bytes.

public class Protocolo
{
    // Largo fijo de la cabecera. Con 4 caracteres, el mensaje mide como maximo 9999.
    public const int LargoCabecera = 4;

    private readonly NetworkDataHelper _red;

    public Protocolo(Socket socket)
    {
        _red = new NetworkDataHelper(socket);
    }

    // ENVIAR: primero el LARGO (4 bytes), despues los DATOS.
    public void EnviarMensaje(string mensaje)
    {
        byte[] datosEnBytes = Encoding.UTF8.GetBytes(mensaje);

        // El largo del mensaje como texto de 4 caracteres: 9 -> "0009".
        string largoTexto = datosEnBytes.Length.ToString().PadLeft(LargoCabecera, '0');
        byte[] largoEnBytes = Encoding.UTF8.GetBytes(largoTexto);

        _red.EnviarTodos(largoEnBytes);  // 4 bytes
        _red.EnviarTodos(datosEnBytes);  // 'largo' bytes
    }

    // RECIBIR: leo el LARGO (4 bytes fijos) y con ese numero leo los DATOS.
    public string RecibirMensaje()
    {
        byte[] largoEnBytes = _red.RecibirTodos(LargoCabecera);
        string largoTexto = Encoding.UTF8.GetString(largoEnBytes);
        int cantidad = int.Parse(largoTexto);

        byte[] datosEnBytes = _red.RecibirTodos(cantidad);
        return Encoding.UTF8.GetString(datosEnBytes);
    }
}
