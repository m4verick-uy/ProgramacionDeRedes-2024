using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DirectvIPTV
{
    public class DirectvSubscriber
    {
        public static async Task Main(string[] args)
        {
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
                _ => "Sin canal asignado"
            };

            if (canal == null)
            {
                Console.WriteLine("Opción inválida. Saliendo...");
                return;
            }

            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("directv", ExchangeType.Direct);
            var queueName = (await channel.QueueDeclareAsync()).QueueName;

            await channel.QueueBindAsync(queue: queueName,
                                         exchange: "directv",
                                         routingKey: canal,
                                         arguments: null);

            Console.WriteLine($" Conectado al canal: {canal.ToUpper()}");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($" {message}");
                await Task.Yield();
            };

            await channel.BasicConsumeAsync(queue: queueName,
                                            autoAck: true,
                                            consumer: consumer);

            Console.WriteLine("Presione [Enter] para salir.");
            Console.ReadLine();
        }
    }
}
