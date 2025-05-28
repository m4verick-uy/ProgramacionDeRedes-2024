using RabbitMQ.Client;
using System.Text;

namespace PrintQueueDemo
{
    public class NewPrint
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "print_queue", durable: true, exclusive: false,
                autoDelete: false, arguments: null);

            Console.WriteLine("=== Sistema de Envío de Trabajos de Impresión ===");
            Console.WriteLine("Ingrese los trabajos (Ctrl+C o vacío para salir).");

            while (true)
            {
                Console.Write("\nNombre del trabajo: ");
                var nombre = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                    break;

                Console.Write("Cantidad de páginas: ");
                if (!int.TryParse(Console.ReadLine(), out int paginas) || paginas < 1)
                {
                    Console.WriteLine("❌ Ingrese un número válido de páginas (>0).");
                    continue;
                }

                string message = $"{nombre}:{paginas}";
                var body = Encoding.UTF8.GetBytes(message);

                var properties = new BasicProperties { Persistent = true };

                await channel.BasicPublishAsync(exchange: string.Empty,
                    routingKey: "print_queue",
                    mandatory: true,
                    basicProperties: properties,
                    body: body);

                Console.WriteLine($" Trabajo enviado: {message}");
            }

            Console.WriteLine("Finalizado. Presione [enter] para salir.");
            Console.ReadLine();
        }
    }
}