using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 1) La primera que responde gana
        Task<string> sucursalA = Consultas.ConsultarAsync("sucursalA", 1500);
        Task<string> sucursalB = Consultas.ConsultarAsync("sucursalB", 800);

        Task<string> ganadora = await Task.WhenAny(sucursalA, sucursalB);
        Console.WriteLine("Respondió primero: {0}", await ganadora);

        // 2) Timeout: la consulta compite contra un Task.Delay
        Task<string> consulta = Consultas.ConsultarAsync("lenta", 5000);
        Task espera = Task.Delay(2000);

        Task primera = await Task.WhenAny(consulta, espera);
        if (primera == consulta)
        {
            Console.WriteLine(await consulta);
        }
        else
        {
            Console.WriteLine("Timeout: no respondió en 2 segundos");
        }
    }
}
