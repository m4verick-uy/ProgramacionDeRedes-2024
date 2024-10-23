using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received: {message}");
        };

        channel.BasicConsume(queue: "publish_subscribe_queue",
            autoAck: true,
            consumer: consumer);

        Console.WriteLine(" [*] Waiting for messages. Press [enter] to exit.");
        Console.ReadLine();
    }
}