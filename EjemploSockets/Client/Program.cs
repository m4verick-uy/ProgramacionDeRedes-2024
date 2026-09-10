namespace Client;

using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Iniciando aplicación cliente !!");

        var socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        socketClient.Bind(localEndPoint);
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
        socketClient.Connect(remoteEndPoint);

        Console.WriteLine("Conectado al servidor !!!");
        Console.WriteLine("Escriba un mensaje para enviar: ");

        bool clienteEstaCorriendo = true;
        while (clienteEstaCorriendo)
        {
            string mensaje = Console.ReadLine();
            if (mensaje == "exit")
            {
                clienteEstaCorriendo = false;
            }
            else
            {
                // ===== CASO 1 - SIN CONTROL DE LARGO =====
                //byte[] mensajeByts = Encoding.UTF8.GetBytes(mensaje);
                //socketClient.Send(mensajeByts);

                // ===== CASO 2 - LARGO COMO TEXTO ("0007") =====

                //   --- 2a: CON BUG (mide en CARACTERES) ---
                //   "mañana" -> Length = 6, pero el cuerpo son 7 bytes -> se rompe
                // int largo = mensaje.Length;                            // 6 (MAL)
                // string largoTexto = largo.ToString("D4");              // "0006"
                // byte[] largoByts = Encoding.UTF8.GetBytes(largoTexto);
                // byte[] mensajeByts = Encoding.UTF8.GetBytes(mensaje);  // 7 bytes
                // socketClient.Send(largoByts);
                // socketClient.Send(mensajeByts);

                //   --- 2b: CORREGIDO (mide en BYTES) ---
                // byte[] mensajeByts = Encoding.UTF8.GetBytes(mensaje);    // primero a bytes
                // int largo = mensajeByts.Length;                          // 7 (BIEN)
                // string largoTexto = largo.ToString("D4");                // "0007"
                // byte[] largoByts = Encoding.UTF8.GetBytes(largoTexto);
                // socketClient.Send(largoByts);
                // socketClient.Send(mensajeByts);

                // ===== CASO 3 - LARGO COMO ENTERO (Int32 binario) =====
                byte[] mensajeByts = Encoding.UTF8.GetBytes(mensaje);
                int largo = mensajeByts.Length;
                byte[] largoByts = BitConverter.GetBytes(largo);       // 4 bytes binarios
                
                socketClient.Send(largoByts);
                socketClient.Send(mensajeByts);
            }
        }

        socketClient.Shutdown(SocketShutdown.Both);
        socketClient.Close();
    }
}