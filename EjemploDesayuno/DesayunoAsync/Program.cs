using Common;

namespace DesayunoAsync
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            DateTime horaInicio = ObtenerTiempo();

            Cafe taza = ServirCafe();
            Console.WriteLine("El café está pronto");

            Huevo huevo = await FreirHuevosAsync(2);
            Console.WriteLine("Los huevos están prontos");

            Panceta panceta = await FreirPancetaAsync(3);
            Console.WriteLine("La panceta está pronta");

            Tostada tostada = await TostarPanAsync(2);
            PonerManteca(tostada);
            PonerMermelada(tostada);
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

        static void PonerManteca(Tostada tostada) // 0.1 segundos
        {
            Console.WriteLine("Poniendo manteca en el pan...");
            Task.Delay(100).Wait();
            tostada.TieneManteca = true;
        }

        static void PonerMermelada(Tostada tostada) // 0.1 segundos
        {
            Console.WriteLine("Poniendo mermelada en el pan...");
            Task.Delay(100).Wait();
            tostada.TieneMermelada = true;
        }

        static Jugo ServirJugo() // 1 segundo
        {
            Console.WriteLine("Sirviendo jugo...");
            Task.Delay(1000).Wait();
            return new Jugo();
        }
    }
}