using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        int cantidad = args.Length > 0 ? int.Parse(args[0]) : 3;
        Stopwatch reloj = Stopwatch.StartNew();

        List<Task<string>> consultas = new List<Task<string>>();
        for (int i = 1; i <= cantidad; i++)
        {
            consultas.Add(Consultas.ConsultarAsync("consulta" + i, 2000));
        }

        string[] respuestas = await Task.WhenAll(consultas);

        foreach (string respuesta in respuestas)
        {
            Console.WriteLine(respuesta);
        }
        Console.WriteLine("Total: {0} ms", reloj.ElapsedMilliseconds);
    }
}
