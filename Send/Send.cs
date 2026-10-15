using RabbitMQ.Client;
using System.Text;

class Send
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "hello",       //especifico el nombre de la cola
            durable: true,        //uso exchange direct
            exclusive: false,     //no sobrevive a un reinicio del servidor, la cola se destruye 
            autoDelete: false,    //no quiero que solo un consumidor pueda conectarse a la cola
            arguments: null);

        const string message = "Hello World!";
        var body = Encoding.UTF8.GetBytes(message);   // Similar a la codificación de los send de sockets

        await channel.BasicPublishAsync(exchange: string.Empty, // tipo de exchange es el Direct 
            routingKey: "hello",    // especifico la cola por su nombre
            body: body);            // el mensaje en bytes
        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}

