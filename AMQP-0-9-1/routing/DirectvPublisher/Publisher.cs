using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.ExchangeDeclareAsync(exchange: "directv", type: ExchangeType.Direct);

Console.WriteLine("📡 DirecTV IPTV — Iniciando transmisión por canales...\n");

await PublishChannel("noticias", new[]
{
    "Científicos crean plástico biodegradable en 72h",
    "NASA limpia basura espacial con láseres",
    "Estudiantes uruguayos ganan mundial de robótica",
    "Bosques del Amazonas muestran recuperación",
    "Avance en vacuna contra Alzheimer"
});

await PublishChannel("deportes", new[]
{
    "⚽ 11:00 - Uruguay vs Francia",
    "⚽ 13:00 - Uruguay vs México",
    "⚽ 16:00 - Uruguay vs Ghana (penales)",
    "⚽ 19:00 - Semifinal: Uruguay vs Alemania",
    "⚽ 21:00 - Final: Uruguay vs España"
});

await PublishChannel("dibujos", new[]
{
    "09:00 - ThunderCats",
    "10:00 - Osos Gummy",
    "11:00 - Star Wars Lego",
    "12:00 - Astroboy",
    "13:00 - Street Fighter"
});

Console.WriteLine("\n📡 Transmisión finalizada. Presione [Enter] para salir.");
Console.ReadLine();

async Task PublishChannel(string routingKey, string[] messages)
{
    foreach (var mensaje in messages)
    {
        var body = Encoding.UTF8.GetBytes(mensaje);
        await channel.BasicPublishAsync(exchange: "directv", routingKey: routingKey, body: body);
        Console.WriteLine($"[→ {routingKey.ToUpper()}] {mensaje}");
        await Task.Delay(500);
    }
}