using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(queue: "task_queue", durable: false, exclusive: false, autoDelete: false,
    arguments: null);

Console.WriteLine(" [x] Escribí una tarea y presioná Enter para enviarla. Cada punto = 1 seg de trabajo. Escribí 'salir' para terminar.");

while (true)
{
    Console.Write(" > ");
    var input = Console.ReadLine();

    if (input.Trim().ToLower() == "salir")
        break;

    var body = Encoding.UTF8.GetBytes(input);
    await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "task_queue",
        mandatory: false, body: body);

    Console.WriteLine($" [x] Enviado: '{input}'");
}

Console.WriteLine(" Saliendo.");