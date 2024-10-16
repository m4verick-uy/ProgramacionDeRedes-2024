using Common;

namespace DesayunoAsync2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            DateTime horaInicio = ObtenerTiempo();
            
            // Lanzamos las tareas asincrónicas en paralelo
            var preparandoHuevosTask = FreirHuevosAsync(2);
            var friendoPancetaTask = FreirPancetaAsync(3);
            var tostandoPanTask = TostarPanAsync(2);
            var jugoTask = ServirJugo();
            var servirCafeTask = ServirCafe();

            var tareasDelDesayuno = new List<Task> { preparandoHuevosTask, friendoPancetaTask, tostandoPanTask, jugoTask, servirCafeTask };

            while (tareasDelDesayuno.Count > 0)
            {
                var tareaTerminada = await Task.WhenAny(tareasDelDesayuno);

                if (tareaTerminada == friendoPancetaTask)
                {
                    Console.WriteLine("La panceta está pronta, {0}", ObtenerTiempo());
                }
                else if (tareaTerminada == preparandoHuevosTask)
                {
                    Console.WriteLine("Los huevos están prontos, {0}", ObtenerTiempo());
                }
                else if (tareaTerminada == tostandoPanTask)
                {
                    Console.WriteLine("Las tostadas están prontas, {0}", ObtenerTiempo());
                }
                else if (tareaTerminada == jugoTask)
                {
                    Console.WriteLine("El jugo de naranja está pronto, {0}", ObtenerTiempo());
                }
                else if (tareaTerminada == servirCafeTask)
                {
                    Console.WriteLine("El café está pronto, {0}", ObtenerTiempo());
                }

                tareasDelDesayuno.Remove(tareaTerminada);
            }

            Console.WriteLine("¡El desayuno está servido!");
            DateTime horaFinalizacion = DateTime.Now;
            TimeSpan duracion = horaFinalizacion - horaInicio;
            Console.WriteLine($"Duración total: {duracion.TotalSeconds} segundos");
        }

        static DateTime ObtenerTiempo()
        {
            DateTime horaInicio = DateTime.Now;
            //Console.WriteLine($"Hora de inicio: {horaInicio}");
            return horaInicio;
        }

        static async Task<Cafe> ServirCafe() // 2 segundos
        {
            Console.WriteLine("Sirviendo café...");
            await Task.Delay(2000); // Simulando una operación síncrona para simplificar
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
        
        static async Task<Jugo> ServirJugo() // 1 segundo
        {
            Console.WriteLine("Sirviendo jugo...");
            await Task.Delay(1000);
            return new Jugo();
        }
    }
}