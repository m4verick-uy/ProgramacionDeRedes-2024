using System;
using System.Threading;
using System.Threading.Tasks;

class RoadTripUSA
{
    // Simula un viaje en auto por varias ciudades de EE.UU.
    static async Task ViajePorEEUUAsync(CancellationToken token)
    {
        string[] ciudades = { 
            "Los Ángeles 🌴", 
            "el Puente de San Francisco 🌉", 
            "Las Vegas 🎰", 
            "el Gran Cañón 🏜️", 
            "Houston 🚀", 
            "Nueva Orleans 🎷", 
            "Miami 🌞", 
            "Nueva York 🗽", 
            "Washington D.C. 🏛️", 
            "Chicago 🌆" 
        };

        Console.WriteLine("¡El road trip por EE.UU. ha comenzado! 🚗💨");

        for (int i = 0; i < ciudades.Length; i++)  // Simulo el viaje por 10 ciudades
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("¡El viaje ha sido cancelado antes de llegar al destino final! 🚨");
                return; // Finaliza la tarea
            }
            
            Console.WriteLine($"Viajando por {ciudades[i]}...");
            await Task.Delay(2000); // Simula un segundo de viaje entre cada ciudad
        }

        Console.WriteLine("¡Has llegado al final del road trip! 🎉");
    }

    static async Task Main(string[] args)
    {
        // Crear el CancellationTokenSource manualmente
        CancellationTokenSource cts = new CancellationTokenSource();

        // Inicio la tarea del road trip
        Task tareaViaje = ViajePorEEUUAsync(cts.Token);

        try
        {
            Console.WriteLine("Escribe 'cancelar' para detener el viaje en cualquier momento.");

            // Bucle que espera hasta que el usuario escriba 'cancelar'
            string input = string.Empty;
            while (input.ToLower() != "cancelar")
            {
                input = Console.ReadLine();
            }


            Console.WriteLine("Decides cancelar el viaje. 😬");
            cts.Cancel(); // Cancela la tarea
            
            await tareaViaje;
        }
        finally
        {
            // Libero recursos manualmente llamando a Dispose
            cts.Dispose();
        }

        Console.WriteLine("Fin del programa.");
    }
}
