using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel); // Defino el consumidor de mensajes
consumer.Received += (model, ea) =>                // expresion lambda que se ejecuta al recibir un mensaje 
{
    var body = ea.Body.ToArray();                   
    var message = Encoding.UTF8.GetString(body);   
    Console.WriteLine($" [x] Received {message}");
    
    int dots = message.Split('.').Length - 1; // Simulo que trabajo 
    Thread.Sleep(dots * 1000);                // más tiempo en tareas

    Console.WriteLine(" [x] Done");
};
channel.BasicConsume(                        // Consumo mensajes
    queue: "hello",                          // de la cola hello
    autoAck: true,                           // y los elimino
    consumer: consumer);                     // automáticamente

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();