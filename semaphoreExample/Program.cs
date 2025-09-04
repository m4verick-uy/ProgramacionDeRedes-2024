class Program
{
    // Creamos un Semaphore que permite un máximo de 3 hilos en la sección crítica a la vez.
    private static Semaphore semaphore = new Semaphore(3, 3);

    static void Main(string[] args)
    {
        // Creamos y lanzamos 10 hilos que intentarán acceder a la sección crítica.
        for (int i = 1; i <= 10; i++)
        {

            Thread thread = new Thread(AccessResource);
            thread.Name = $"Hilo {i}";
            thread.Start();
        }

        Console.ReadLine(); // Esperamos para que la consola no se cierre inmediatamente.
    }

    private static void AccessResource()
    {
        Console.WriteLine($"{Thread.CurrentThread.Name} está esperando para entrar...");

        // Intentamos entrar en la sección crítica.
        semaphore.WaitOne(); // Adquirimos un "permiso" del semaphore.

        try
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} ha entrado en la sección crítica.");

            // Simulamos un trabajo dentro de la sección crítica.
            Thread.Sleep(1000);
        }
        finally
        {
            // Liberamos el permiso para que otro hilo pueda acceder.
            semaphore.Release();
            Console.WriteLine($"{Thread.CurrentThread.Name} ha salido de la sección crítica.");
        }
    }
}