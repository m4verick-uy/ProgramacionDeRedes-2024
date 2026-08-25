using System.Threading;

class Program
{
    static void Main()
    {
        Thread hilo1 = new Thread(() => ProcesarArchivo("clientes.txt"));
        Thread hilo2 = new Thread(() => ProcesarArchivo("ventas.txt"));

        hilo1.Start();
        hilo2.Start();

        hilo1.Join();
        hilo2.Join();

        Console.WriteLine("Procesamiento finalizado.");
    }

    static void ProcesarArchivo(string archivo)
    {
        try
        {
            Console.WriteLine($"Procesando {archivo}...");

            if (archivo == "ventas.txt")
            {
                throw new Exception("No se pudo leer el archivo");
            }

            Thread.Sleep(2000);

            Console.WriteLine($"{archivo} procesado correctamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error procesando {archivo}: {ex.Message}");
        }
    }
}