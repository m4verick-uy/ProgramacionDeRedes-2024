using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQExamples
{
    public class Send
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            for (int i = 1; i <= 10000000; i++)
            {
                string message = $"Mensaje #{i}";
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
                Console.WriteLine($" [x] Sent {message}");

                await Task.Delay(50); // Simula una pausa para observar la cola crecer
            }

            Console.WriteLine(" Presioná Enter para salir.");
            Console.ReadLine();
        }
    }
}