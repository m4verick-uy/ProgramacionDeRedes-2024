using System.Net.NetworkInformation;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Creamos la conexion al servidor de mensajeria

var factory = new ConnectionFactory { HostName = "localhost"};
using var connnection = factory.CreateConnection(); // creamos una conexion
using var channel = connnection.CreateModel(); // creamos un canal para definir la cola de mensajes

// Delete the existing queue
channel.QueueDelete(queue: "hello");

channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

Console.WriteLine("[*] Waiting for menssages.");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) => //es el proceso de desencolar el mensaje 
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body); // el mensaje lo traducimos de bytes a string para mostrar
    Console.WriteLine($"[x] Received {message}"); // mostrmos el mensaje recibido
};

channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

Console.WriteLine("Press [enter] to exit.");
Console.ReadLine();

