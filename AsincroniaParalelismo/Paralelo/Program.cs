using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    const int Limite = 3000000;

    static void Main(string[] args)
    {
        Stopwatch reloj = Stopwatch.StartNew();

        // 1) Secuencial
        int secuencial = 0;
        for (int i = 2; i < Limite; i++)
        {
            if (EsPrimo(i)) secuencial++;
        }
        Console.WriteLine("For:               {0} primos en {1} ms",
            secuencial, reloj.ElapsedMilliseconds);

        // 2) Parallel.For SIN protección: condición de carrera
        reloj.Restart();
        int roto = 0;
        Parallel.For(2, Limite, i =>
        {
            if (EsPrimo(i)) roto++;
        });
        Console.WriteLine("Parallel.For roto: {0} primos en {1} ms",
            roto, reloj.ElapsedMilliseconds);

        // 3) Parallel.For con lock
        reloj.Restart();
        object candado = new object();
        int conLock = 0;
        Parallel.For(2, Limite, i =>
        {
            if (EsPrimo(i))
            {
                lock (candado)
                {
                    conLock++;
                }
            }
        });
        Console.WriteLine("Parallel.For lock: {0} primos en {1} ms",
            conLock, reloj.ElapsedMilliseconds);

        // 4) Parallel.ForEach sobre una colección
        reloj.Restart();
        int[] numeros = Enumerable.Range(2, Limite - 2).ToArray();
        int conForEach = 0;
        Parallel.ForEach(numeros, n =>
        {
            if (EsPrimo(n))
            {
                lock (candado)
                {
                    conForEach++;
                }
            }
        });
        Console.WriteLine("Parallel.ForEach:  {0} primos en {1} ms",
            conForEach, reloj.ElapsedMilliseconds);

        // 5) PLINQ: sin locks, el framework combina los resultados
        reloj.Restart();
        int conPlinq = numeros.AsParallel().Count(n => EsPrimo(n));
        Console.WriteLine("PLINQ:             {0} primos en {1} ms",
            conPlinq, reloj.ElapsedMilliseconds);
    }

    static bool EsPrimo(int n)
    {
        if (n < 2) return false;
        for (int d = 2; (long)d * d <= n; d++)
        {
            if (n % d == 0) return false;
        }
        return true;
    }
}
