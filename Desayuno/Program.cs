using System;
using System.Diagnostics;
using System.Threading;

class Coffee { }
class Egg { }
class Bacon { }
class Toast { }
class Juice { }

class Program
{
    static void Main(string[] args)
    {
        Stopwatch reloj = Stopwatch.StartNew();

        Coffee cup = PourCoffee();
        Console.WriteLine("coffee is ready");

        Egg eggs = FryEggs(2);
        Console.WriteLine("eggs are ready");

        Bacon bacon = FryBacon(3);
        Console.WriteLine("bacon is ready");

        Toast toast = ToastBread(2);
        ApplyButter(toast);
        ApplyJam(toast);
        Console.WriteLine("toast is ready");

        Juice oj = PourOJ();
        Console.WriteLine("oj is ready");
        Console.WriteLine("Breakfast is ready!");

        Console.WriteLine("Total: {0} ms", reloj.ElapsedMilliseconds);
    }

    static Juice PourOJ()
    {
        Console.WriteLine("Pouring orange juice");
        return new Juice();
    }

    static void ApplyJam(Toast toast)
    {
        Console.WriteLine("Putting jam on the toast");
    }

    static void ApplyButter(Toast toast)
    {
        Console.WriteLine("Putting butter on the toast");
    }

    static Toast ToastBread(int slices)
    {
        for (int slice = 0; slice < slices; slice++)
        {
            Console.WriteLine("Putting a slice of bread in the toaster");
        }
        Console.WriteLine("Start toasting...");
        Thread.Sleep(3000);
        Console.WriteLine("Remove toast from toaster");

        return new Toast();
    }

    static Bacon FryBacon(int slices)
    {
        Console.WriteLine("putting {0} slices of bacon in the pan", slices);
        Console.WriteLine("cooking first side of bacon...");
        Thread.Sleep(3000);
        for (int slice = 0; slice < slices; slice++)
        {
            Console.WriteLine("flipping a slice of bacon");
        }
        Console.WriteLine("cooking the second side of bacon...");
        Thread.Sleep(3000);
        Console.WriteLine("Put bacon on plate");

        return new Bacon();
    }

    static Egg FryEggs(int howMany)
    {
        Console.WriteLine("Warming the egg pan...");
        Thread.Sleep(3000);
        Console.WriteLine("cracking {0} eggs", howMany);
        Console.WriteLine("cooking the eggs ...");
        Thread.Sleep(3000);
        Console.WriteLine("Put eggs on plate");

        return new Egg();
    }

    static Coffee PourCoffee()
    {
        Console.WriteLine("Pouring coffee");
        return new Coffee();
    }
}