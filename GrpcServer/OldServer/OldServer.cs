using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GrpcServer.OldServer
{
    public class OldServer
    {
        public static async Task StartOldServer()
        {
            Console.WriteLine("Iniciando app Servidor.....");

            var ipStr = System.Configuration.ConfigurationManager.AppSettings["ServerIP"] ?? "0.0.0.0";
            var portStr = System.Configuration.ConfigurationManager.AppSettings["ServerPort"] ?? "10000";

            var ip = IPAddress.Parse(ipStr);
            var port = int.Parse(portStr);

            var socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketServidor.Bind(new IPEndPoint(ip, port));
            socketServidor.Listen(10);

            Console.WriteLine($"Servidor escuchando en {ip}:{port}");

            while (true)
            {
                var socketCliente = await socketServidor.AcceptAsync();
                Console.WriteLine("Client connected");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        var buffer = new byte[1024];
                        while (socketCliente.Connected) // Mantener conexión mientras el cliente esté conectado
                        {
                            int bytesRecibidos = await socketCliente.ReceiveAsync(buffer, SocketFlags.None);
                            if (bytesRecibidos == 0) // Cliente cerró la conexión
                            {
                                Console.WriteLine("Cliente desconectado");
                                break;
                            }

                            var mensaje = Encoding.UTF8.GetString(buffer, 0, bytesRecibidos);
                            Console.WriteLine($"Recibido: {mensaje}");

                            // Guardar el mensaje en un archivo .txt en /app/images
                            string imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "images");
                            string fileName = $"mensaje_{DateTime.Now.Ticks}.txt";
                            string filePath = Path.Combine(imagesDir, fileName);

                            Directory.CreateDirectory(imagesDir); // Asegurarse de que el directorio exista
                            await File.WriteAllTextAsync(filePath, mensaje); // Escribe el mensaje en el archivo

                            Console.WriteLine($"Mensaje guardado en: {filePath}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar datos: {ex.Message}");
                    }
                    finally
                    {
                        if (socketCliente.Connected)
                        {
                            socketCliente.Shutdown(SocketShutdown.Both);
                        }

                        socketCliente.Close();
                        Console.WriteLine("Conexión cerrada");
                    }
                });
            }
        }
    }
}