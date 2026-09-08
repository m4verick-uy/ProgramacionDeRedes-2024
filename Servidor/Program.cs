using Servidor;
using Servidor.Almacenamiento;
using Servidor.Logica;

Console.WriteLine("=== Servidor Edison ===");

// "Base de datos" en memoria + lógica de negocio.
var almacen = new Almacen();
var logica = new LogicaNegocio(almacen);
var servidor = new ServidorTcp(almacen, logica);

// El bucle de aceptación corre en un Thread aparte para que el Main pueda
// quedar leyendo la consola y ofrecer el cierre controlado (SR4).
var hiloServidor = new Thread(servidor.Iniciar);
hiloServidor.Start();

Console.WriteLine("Escriba 'exit' y Enter para cerrar el servidor de forma controlada.");

bool cerrar = false;
while (!cerrar)
{
    string? linea = Console.ReadLine();

    // Ctrl+D / EOF en la consola devuelve null: también cerramos.
    if (linea == null || linea.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        cerrar = true;
    else
        Console.WriteLine("Comando no reconocido. Escriba 'exit' para cerrar.");
}

// Cierre controlado: dejar de aceptar y liberar el socket de escucha.
servidor.Detener();
hiloServidor.Join();

Console.WriteLine("[Servidor] Finalizado.");
