using Common;

namespace Desayuno
{
    internal class Program
    {
        static void Main(string[] args) // 13.1 segundos aprox
        {
            DateTime horaInicio = ObtenerTiempo();

            Cafe taza = ServirCafe();
            Console.WriteLine("El café está pronto");

            Huevo huevo = FreirHuevos(2);
            Console.WriteLine("Los huevos están prontos");

            Panceta panceta = FreirPanceta(3);
            Console.WriteLine("La panceta está pronta");

            Tostada tostada = TostarPan(2);
            PonerManteca(tostada);
            PonerMermelada(tostada);
            Console.WriteLine("Las tostadas están prontas");

            Jugo jugoNaranja = ServirJugo();
            Console.WriteLine("El juego de naranja está pronto");
            Console.WriteLine();
            Console.Write("¡El desayuno está servido!");
            DateTime horaFinalizacion = DateTime.Now;
            Console.WriteLine();
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
            Thread.Sleep(2000);
            return new Cafe();
        }

        static Huevo FreirHuevos(int cantidad) // 5 segundos
        {
            Console.WriteLine($"Calentando la sartén...");
            Thread.Sleep(3000);

            Console.WriteLine($"Rompiendo {cantidad} huevos...");
            Thread.Sleep(1000);

            Console.WriteLine($"Fritando los huevos...");
            Thread.Sleep(2000);

            return new Huevo(cantidad);
        }

        static Panceta FreirPanceta(int fetas) // 1.5 segundos
        {
            Console.WriteLine("Fritando panceta...");
            for (int feta = 1; feta <= fetas; feta++)
            {
                Console.WriteLine($"Fritando feta {feta}...");
                Thread.Sleep(500);
            }
            
            Console.WriteLine("Poniendo panceta en el plato...");

            return new Panceta(fetas);
        }

        static Tostada TostarPan(int cantidad) // 3.4 segundos
        {
            for (int pan = 0; pan < cantidad; pan++)
            {
                Console.WriteLine("Poniendo un pan en la tostadora...");
                Thread.Sleep(200);
            }
            Console.WriteLine("Comienza el tostado de pan...");
            Thread.Sleep(3000);
            Console.WriteLine("Sacando pan de la tostadora...");

            return new Tostada(cantidad);
        }

        static void PonerManteca(Tostada tostada) // 0.1 segundos
        {
            Console.WriteLine("Poniendo manteca en el pan...");
            Thread.Sleep(100);
            tostada.TieneManteca = true;
        }

        static void PonerMermelada(Tostada tostada) // 0.1 segundos
        {
            Console.WriteLine("Poniendo mermelada en el pan...");
            Thread.Sleep(100);
            tostada.TieneMermelada = true;
        }

        static Jugo ServirJugo() // 1 segundo
        {
            Console.WriteLine("Sirviendo jugo...");
            Thread.Sleep(1000);
            return new Jugo();
        }
    }
}