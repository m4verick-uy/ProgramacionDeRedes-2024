using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(  //metodo para declarar una cola
    queue: "hello",    //especifico el nombre de la cola
    durable: false,    //no sobrevive a un reinicio del servidor, la cola se destruye 
    exclusive: false,  //no quiero que solo un consumidor pueda conectarse a la cola
    autoDelete: false, //no se elimina la cola cuando no haya consumidores
    arguments: null);  //para no pasar argumentos adicionales

bool salir = false;
while (!salir)
{
    Console.WriteLine("Escribe un mensaje: ");
    string message = Console.ReadLine();
    var body = Encoding.UTF8.GetBytes(message);
    
    channel.BasicPublish(       //metodo para publicar un mensaje    
        exchange: string.Empty, //uso exchange direct
        routingKey: "hello",    //especifico la cola por su nombre
        basicProperties: null,  //no quiero pasar propiedades adicionales
        body: body);            //el mensaje en bytes
    
    Console.WriteLine($" [x] Enviando: {message}");
    if (message == "exit")
    {
        salir = true;
    }
}

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();