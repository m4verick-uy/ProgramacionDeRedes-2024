using Common;

namespace DesayunoAsync2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            DateTime horaInicio = ObtenerTiempo();

            Cafe taza = ServirCafe();
            Console.WriteLine("El café está pronto");
            
            // Lanzamos las tareas asincrónicas en paralelo
            Task<Huevo> preparandoHuevosTask = FreirHuevosAsync(2);
            Task<Panceta> friendoPancetaTask = FreirPancetaAsync(3);
            Task<Tostada> tostandoPanTask = TostarPanAsync(2);

            
            var eggs = await preparandoHuevosTask;
            Console.WriteLine("Los huevos están prontos");

            var bacon = await friendoPancetaTask;
            Console.WriteLine("La panceta está pronta");

            var toast = await tostandoPanTask;
            Console.WriteLine("Las tostadas están prontas");
            
            Jugo jugoNaranja = ServirJugo();
            Console.WriteLine("El jugo de naranja está pronto");
            Console.WriteLine("¡El desayuno está servido!");

            DateTime horaFinalizacion = DateTime.Now;
            TimeSpan duracion = horaFinalizacion - horaInicio;
            Console.WriteLine($"Duración total: {duracion.TotalSeconds} segundos");
        }

        static DateTime ObtenerTiempo()
        {
            DateTime horaInicio = DateTime.Now;
            Console.WriteLine($"Hora de inicio: {horaInicio}");
            return horaInicio;
        }

        static Cafe ServirCafe() // 2 segundos
        {
            Console.WriteLine("Sirviendo café...");
            Task.Delay(2000).Wait(); // Simulando una operación síncrona para simplificar
            return new Cafe();
        }

        static async Task<Huevo> FreirHuevosAsync(int cantidad) // 5 segundos
        {
            Console.WriteLine($"Calentando la sartén...");
            await Task.Delay(3000);

            Console.WriteLine($"Rompiendo {cantidad} huevos...");
            await Task.Delay(1000);

            Console.WriteLine($"Fritando los huevos...");
            await Task.Delay(2000);

            return new Huevo(cantidad);
        }

        static async Task<Panceta> FreirPancetaAsync(int fetas) // 1.5 segundos
        {
            Console.WriteLine("Fritando panceta...");
            for (int feta = 1; feta <= fetas; feta++)
            {
                Console.WriteLine($"Fritando feta {feta}...");
                await Task.Delay(500);
            }

            Console.WriteLine("Poniendo panceta en el plato...");
            return new Panceta(fetas);
        }

        static async Task<Tostada> TostarPanAsync(int cantidad) // 3.4 segundos
        {
            for (int pan = 0; pan < cantidad; pan++)
            {
                Console.WriteLine("Poniendo un pan en la tostadora...");
                await Task.Delay(200);
            }
            Console.WriteLine("Comienza el tostado de pan...");
            await Task.Delay(3000);
            Console.WriteLine("Sacando pan de la tostadora...");

            return new Tostada(cantidad);
        }
        
        static Jugo ServirJugo() // 1 segundo
        {
            Console.WriteLine("Sirviendo jugo...");
            Task.Delay(1000).Wait();
            return new Jugo();
        }
    }
}