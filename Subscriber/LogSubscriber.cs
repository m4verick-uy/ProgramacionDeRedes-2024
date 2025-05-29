using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    public class LogSubscriber
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

            // Cola con nombre generado por el servidor
            var queueDeclare = await channel.QueueDeclareAsync();
            string queueName = queueDeclare.QueueName;

            await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: "");

            Console.WriteLine(" [*] Esperando logs. Presione Ctrl+C para salir.");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Log recibido: {message}");
                await Task.Yield(); // evitar warning
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
            Console.ReadLine();
        }
    }
}