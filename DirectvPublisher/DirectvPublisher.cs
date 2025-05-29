using System.Text;
using RabbitMQ.Client;

namespace DirectvIPTV
{
    public class DirectvPublisher
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("directv", ExchangeType.Direct);

            await PublishChannel("noticias", new[]
            {
                "Bienvenido al canal NOTICIAS",
                "Científicos crean plástico biodegradable en 72h",
                "NASA limpia basura espacial con láseres",
                "Estudiantes uruguayos ganan mundial de robótica",
                "Bosques del Amazonas muestran recuperación",
                "Avance en vacuna contra Alzheimer"
            });

            await PublishChannel("deportes", new[]
            {
                "Bienvenido al canal DEPORTES",
                "11:00 - Uruguay vs Francia",
                "13:00 - Uruguay vs México",
                "16:00 - Uruguay vs Ghana (penales)",
                "19:00 - Semifinal: Uruguay vs Alemania",
                "21:00 - Final: Uruguay vs España"
            });

            await PublishChannel("dibujos", new[]
            {
                "Bienvenido al canal DIBUJOS ANIMADOS",
                "09:00 - ThunderCats",
                "10:00 - Osos Gummy",
                "11:00 - Star Wars Lego",
                "12:00 - Astroboy",
                "13:00 - Street Fighter"
            });

            async Task PublishChannel(string routingKey, string[] messages)
            {
                foreach (var message in messages)
                {
                    var body = Encoding.UTF8.GetBytes(message);
                    await channel.BasicPublishAsync(
                        exchange: "directv",
                        routingKey: routingKey,
                        body: body
                    );
                    
                    Console.WriteLine($"[→ {routingKey.ToUpper()}] {message}");
                    await Task.Delay(1000); // Simula transmisión en tiempo real
                }
            }

            Console.WriteLine("📡 Transmisión finalizada. Presione [Enter] para salir.");
            Console.ReadLine();
        }
    }
}
