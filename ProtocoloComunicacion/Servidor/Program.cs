using System.Net;
using System.Net.Sockets;
using CommonLibrary;

namespace Servidor;


//  - LARGO(4) + DATOS   (modelo "9999", solo largo + datos)
//  - Acepta varios clientes (un thread por cliente).
//  - Al conectar un cliente, le manda "Bienvenido al servidor".
//  - Recibe cada mensaje del cliente y responde "OK" (request/response).
//  - Cierre controlado: se escribe "exit" y deja de aceptar clientes.


public class Program
{
    private const int Puerto = 20000;
    private static bool _enEjecucion = true;
    private static Socket _socketServidor = null!;

    public static void Main()
    {
        _socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socketServidor.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Puerto));
        _socketServidor.Listen(10);

        Console.WriteLine($"[Servidor] Escuchando en 127.0.0.1:{Puerto}");
        Console.WriteLine("[Servidor] Escriba 'exit' para cerrar.");

        // El bucle de Accept corre en un thread aparte; el Main queda leyendo la
        // consola para poder ordenar el cierre.
        Thread hiloAceptacion = new Thread(BucleAceptacion);
        hiloAceptacion.IsBackground = true;   //// cambio aca /////
        hiloAceptacion.Start();
        
        
        // Posible manejo del Main y cierre
        string linea = "";
        while (linea != "exit")
        {
            linea = Console.ReadLine();
            if (linea == null)          // Ctrl+D / entrada cerrada: tratamos como salir
                linea = "exit";
            linea = linea.ToLower();    // "EXIT", "Exit" -> "exit"
        }

        // Cierre controlado: dejo de aceptar y cierro el socket de escucha.
        // (No cerramos aqui los sockets de los clientes ya conectados: cada
        //  HandleClient maneja su propia desconexion. Se explica en el teorico.)
        _enEjecucion = false;
        try
        {
            _socketServidor.Close();
        }
        catch (SocketException)
        { /* el socket ya estaba cerrado, no hay nada que hacer */ } // desbloquea el Accept()
        
        Console.WriteLine("[Servidor] Cerrado.");
    }

    private static void BucleAceptacion()
    {
        while (_enEjecucion)
        {
            try
            {
                Socket socketCliente = _socketServidor.Accept(); // Es bloqueante
                Console.WriteLine("[Servidor] Cliente conectado.");
                
                // Un thread por cliente: varios clientes a la vez sin bloquearse.
                Thread hilo = new Thread(() => HandleClient(socketCliente));
                hilo.IsBackground = true; //// CAMBIO ACA ///////
                hilo.Start();
            }
            catch (SocketException)
            {
                // Al cerrar el socket de escucha, Accept lanza SocketException.
                // Si fue cierre pedido, salimos del bucle.
                if (!_enEjecucion) break;
            }
        }
    }
    
    // Atiende a UN cliente en su propio thread.
    private static void HandleClient(Socket socketCliente)
    {
        Protocolo protocolo = new Protocolo(socketCliente);

        try
        {
            // Apenas se conecta, le mando la bienvenida (dentro del try).
            protocolo.EnviarMensaje("Bienvenido al servidor");

            // Recibo cada mensaje del cliente y respondo "OK", hasta que se va.
            bool clienteConectado = true;
            while (clienteConectado)
            {
                string mensaje = protocolo.RecibirMensaje();   // request del cliente
                Console.WriteLine("[Servidor] Cliente dice: " + mensaje);
                protocolo.EnviarMensaje("OK");                 // response al cliente
            }
        }
        catch (SocketException)
        {
            // El cliente se desconecto (Receive devolvio 0). Es lo esperado.
            Console.WriteLine("[Servidor] Cliente desconectado.");
        }
        finally
        {
            // Cierre correcto de mi socket: Shutdown y luego Close.
            try
            {
                socketCliente.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException)
            {
                
            }
            socketCliente.Close();
        }
    }
}
