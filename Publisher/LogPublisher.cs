using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Publisher
{
    public class LogPublisher
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

            var message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: "logs",
                routingKey: string.Empty,
                body: body
            );

            Console.WriteLine($" [x] Sent: {message}");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return args.Length > 0 ? string.Join(" ", args) : "info: Hello World!";
        }
    }
}