using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

Console.WriteLine("=== Bienvenido a DirecTV IPTV ===");
Console.WriteLine("Seleccione un canal:");
Console.WriteLine("1 - Noticias");
Console.WriteLine("2 - Deportes");
Console.WriteLine("3 - Dibujos Animados");
Console.Write("Canal: ");
var input = Console.ReadLine();

string canal = input switch
{
    "1" => "noticias",
    "2" => "deportes",
    "3" => "dibujos",
    _ => null
};

if (canal == null)
{
    Console.WriteLine("Opción inválida. Saliendo...");
    return;
}

await channel.ExchangeDeclareAsync(exchange: "directv", type: ExchangeType.Direct);

var queueDeclareResult = await channel.QueueDeclareAsync();
var queueName = queueDeclareResult.QueueName;

await channel.QueueBindAsync(queue: queueName, exchange: "directv", routingKey: canal);

Console.WriteLine($"📺 Conectado al canal: {canal.ToUpper()}");

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += (model, ea) =>
{
    var mensaje = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine($" {mensaje}");
    return Task.CompletedTask;
};

await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

Console.WriteLine(" Presione [Enter] para salir.");
Console.ReadLine();