//Sender.cs con 10 intentos
using System;
using System.Text;
using RabbitMQ.Client;


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.QueueDeclare(queue: "cola2",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

for (int i = 0; i < 1000000; i++) // Enviar 10 mensajes para verificar el encolamiento
{
    string message = $"Hello World! {i}";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: string.Empty,
        routingKey: "hello",
        basicProperties: null,
        body: body);
    Console.WriteLine($" [x] Sent {message}");
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();