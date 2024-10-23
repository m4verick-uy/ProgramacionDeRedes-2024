using System;
using System.Text;
using RabbitMQ.Client;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "publish_subscribe_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        for (int i = 0; i < 5; i++)
        {
            string message = $"Hello World {i}";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: string.Empty,
                routingKey: "publish_subscribe_queue",
                basicProperties: null,
                body: body);
            Console.WriteLine($" [x] Sent: {message}");
            System.Threading.Thread.Sleep(1000); // Espera un segundo entre envíos
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}