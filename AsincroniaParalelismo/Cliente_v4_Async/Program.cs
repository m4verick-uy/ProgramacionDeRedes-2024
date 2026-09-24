using System;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Stopwatch reloj = Stopwatch.StartNew();

        // A) Esperamos cada consulta antes de lanzar la siguiente
        string a1 = await Consultas.ConsultarAsync("a", 1000);
        string a2 = await Consultas.ConsultarAsync("b", 2000);
        string a3 = await Consultas.ConsultarAsync("c", 3000);
        Console.WriteLine("A) Una tras otra: {0} ms",
            reloj.ElapsedMilliseconds);

        // B) Lanzamos las tres y recién después esperamos
        reloj.Restart();
        Task<string> t1 = Consultas.ConsultarAsync("a", 1000);
        Task<string> t2 = Consultas.ConsultarAsync("b", 2000);
        Task<string> t3 = Consultas.ConsultarAsync("c", 3000);
        string b1 = await t1;
        string b2 = await t2;
        string b3 = await t3;
        Console.WriteLine("B) Lanzadas juntas: {0} ms",
            reloj.ElapsedMilliseconds);
    }
}
