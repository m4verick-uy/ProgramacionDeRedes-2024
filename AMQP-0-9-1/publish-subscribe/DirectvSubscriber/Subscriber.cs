using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "directv", type: ExchangeType.Fanout);

var queueDeclareResult = await channel.QueueDeclareAsync();
var queueName = queueDeclareResult.QueueName;

await channel.QueueBindAsync(queue: queueName, exchange: "directv", routingKey: string.Empty);

Console.WriteLine("📺 DirecTV IPTV — Suscriptor conectado. Esperando transmisión...");
Task.Delay(1000);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var mensaje = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($" {mensaje}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();