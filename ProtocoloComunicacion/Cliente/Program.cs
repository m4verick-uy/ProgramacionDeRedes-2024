using System.Net;
using System.Net.Sockets;
using CommonLibrary;

namespace Cliente;

//  - LARGO(4) + DATOS   (modelo "9999", solo largo + datos)
//  - Se conecta y recibe el "Bienvenido al servidor".
//  - Manda cada mensaje que el usuario escribe y espera la respuesta "OK", hasta que escribe "exit".

public class Program
{
    private const int Puerto = 20000;

    public static void Main()
    {
        Socket socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // Endpoint local con puerto 0: el sistema asigna cualquier puerto
            IPEndPoint endpointLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            IPEndPoint endpointServidor = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Puerto);

            socketCliente.Bind(endpointLocal);
            socketCliente.Connect(endpointServidor);
            Protocolo protocolo = new Protocolo(socketCliente);

            // Recibo el mensaje de bienvenida del servidor.
            string bienvenida = protocolo.RecibirMensaje();
            Console.WriteLine("[Cliente] Servidor dice: " + bienvenida);

            // Mando mensajes y espero la respuesta, hasta escribir "exit".
            Console.WriteLine("[Cliente] Escriba mensajes ('exit' para salir):");
            bool salir = false;
            while (!salir)
            {
                Console.Write("> ");
                string mensaje = Console.ReadLine() ?? "";

                if (mensaje.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    salir = true;
                }
                else
                {
                    protocolo.EnviarMensaje(mensaje);              // envio el mensaje
                    string respuesta = protocolo.RecibirMensaje(); // espero la respuesta
                    Console.WriteLine("[Cliente] Servidor dice: " + respuesta);
                }
            }
        }
        catch (SocketException)
        {
            // No se pudo conectar, o el servidor cerro la conexion mientras
            // trabajabamos. Asi el cliente no queda colgado.
            Console.WriteLine("[Cliente] Se perdio la conexion con el servidor.");
        }
        finally
        {
            // Cierre correcto: Shutdown y luego Close, pase lo que pase.
            try
            {
                socketCliente.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
                
            }
            socketCliente.Close();
            Console.WriteLine("[Cliente] Conexion cerrada.");
        }
    }
}
