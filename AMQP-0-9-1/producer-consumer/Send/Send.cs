using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "hello", durable: true, exclusive: false, autoDelete: false,
    arguments: null);


Console.WriteLine("Write a message to be send or exit to leave");
bool exit  = false;
while (!exit)
{
    Console.Write(">");
    var input = Console.ReadLine();
    if (input == "exit")
    {
        exit = true;
        break;
    }
    else
    {
        var body = Encoding.UTF8.GetBytes(input);
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "hello", body: body);
        Console.WriteLine("Message sent");
    }
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();