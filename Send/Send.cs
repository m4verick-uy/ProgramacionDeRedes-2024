using System.Text;
using RabbitMQ.Client; // importar la librería de RabbitMQ


// Crear la conexión con el servidor de RabbitMQ

var factory = new ConnectionFactory { HostName = "localhost" };

using var connnection = factory.CreateConnection();
using var channel = connnection.CreateModel();

// Crear la cola
channel.QueueDeclare(queue: "hello", // nombre de la cola
                      durable: false, // tras un reset no es necesario volver a crear la cola
                      exclusive: false, //  la cola puede ser usada por cualquier conexión
                      autoDelete: true, // la cola no debe ser eliminada cuando no se use
                      arguments: null); // sirven para configuraciones adicionales


bool exit = false;

while (!exit)
{
    Console.WriteLine("Escribe un mensaje para enviar al servidor RabbitMQ (escribe 'salir' para teminar)");
    string message = Console.ReadLine();
    if (message.ToLower() == "salir")
    {
        exit = true;
    }
    else
    {
        var body = Encoding.UTF8.GetBytes(message); // similar a cuando enviamos un mensaje por socket

        // Publicar el mensaje en la cola

        channel.BasicPublish( exchange: string.Empty, // intercambio por defecto
            routingKey: "hello", // nombre de la cola
            basicProperties: null, // propiedades adicionales
            body: body); // el mensaje a enviar
        
        Console.WriteLine($" [x] Sent {message}");
    }
}


Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();