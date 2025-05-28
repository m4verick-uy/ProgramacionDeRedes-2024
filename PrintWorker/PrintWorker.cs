using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PrintQueueDemo
{
    public class PrintWorker
    {
        public static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: "print_queue", durable: true, exclusive: false,
                autoDelete: false, arguments: null);

            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            string printerName = $"Impresora-{Environment.ProcessId}";
            Console.WriteLine($" [*] {printerName} lista. Esperando trabajos...");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var parts = message.Split(':');
                var jobName = parts[0];
                var pages = int.Parse(parts[1]);

                Console.WriteLine($" [{printerName}] Recibido: {jobName} - {pages} páginas.");
                await Task.Delay(pages * 1000); // Simulo impresión
                Console.WriteLine($" [ {printerName}] {jobName} completado.");

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(queue: "print_queue", autoAck: false, consumer: consumer);

            Console.WriteLine(" Presione [enter] para salir.");
            Console.ReadLine();
        }
    }
}