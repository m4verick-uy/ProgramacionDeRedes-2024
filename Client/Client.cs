using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace Cliente
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando app Cliente...!");

            string ipServer = ConfigurationManager.AppSettings["ServerIP"] ?? "127.0.0.1";
            int port = int.Parse(ConfigurationManager.AppSettings["ServerPort"] ?? "10000");

            var socketCliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketCliente.Bind(new IPEndPoint(IPAddress.Any, 0));

            await socketCliente.ConnectAsync(ipServer, port);
            Console.WriteLine($"Conectado al servidor {ipServer}:{port}");

            // Lógica de envío dentro de una tarea
            var tareaEnvio = Task.Run(async () =>
            {
                while (true)
                {
                    Console.Write("Mensaje: ");
                    string mensaje = Console.ReadLine();
                    if (mensaje?.Trim().ToLower() == "salir") break;

                    byte[] data = Encoding.UTF8.GetBytes(mensaje);
                    await socketCliente.SendAsync(data, SocketFlags.None);
                }

                Console.WriteLine("Cerrando conexión.");
                socketCliente.Shutdown(SocketShutdown.Both);
                socketCliente.Close();
            });

            await tareaEnvio;
        }
    }
}