using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "directv", type: ExchangeType.Fanout);

var noticias = new[]
{
    "Científicos crean plástico biodegradable en 72h",
    "NASA limpia basura espacial con láseres",
    "Estudiantes uruguayos ganan mundial de robótica",
    "Nacional gana la Copa Libertadores",
    "Uruguay a la final contra Brasil en mundial 2030"
};

Console.WriteLine("DirecTV IPTV — Iniciando transmisión...\n");

foreach (var mensaje in noticias)
{
    var body = Encoding.UTF8.GetBytes(mensaje);
    await channel.BasicPublishAsync(exchange: "directv", routingKey: string.Empty, body: body);
    Console.WriteLine($"[→] {mensaje}");
    await Task.Delay(1000);
}

Console.WriteLine("\nTransmisión finalizada. Presione [Enter] para salir.");
Console.ReadLine();