using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        // 1) Tres formas de crear y arrancar una tarea
        Task t1 = new Task(() => Console.WriteLine("t1: new Task + Start"));
        t1.Start();                   // new Task NO arranca sola

        Task t2 = Task.Run(() => Console.WriteLine("t2: Task.Run"));   // ya corre

        Task t3 = Task.Factory.StartNew(() =>
            Console.WriteLine("t3: Task.Factory.StartNew"));

        Task.WaitAll(t1, t2, t3);     // si no esperamos, el Main puede terminar

        // 2) Una tarea que devuelve un valor
        Task<int> suma = Task.Run(() => 40 + 2);
        Console.WriteLine("Resultado: {0}", suma.Result);   // Result espera

        // 3) Delay: el equivalente asincrónico de Thread.Sleep
        Task espera = Task.Delay(1000);   // un temporizador, no bloquea un hilo
        Console.WriteLine("Estado de espera: {0}", espera.Status);
        espera.Wait();
        Console.WriteLine("Estado de espera: {0}", espera.Status);

        // 4) Continuación: qué hacer cuando termina la tarea
        Task<int> calculo = Task.Run(() => 6 * 7);
        Task mostrar = calculo.ContinueWith(anterior =>
            Console.WriteLine("La respuesta es {0}", anterior.Result));
        mostrar.Wait();

        // 5) Una tarea dentro de otra
        Task<Task> externa = Task.Factory.StartNew(async () =>
        {
            await Task.Delay(1000);
            Console.WriteLine("La tarea interna terminó");
        });
        externa.Wait();
        Console.WriteLine("La externa terminó... ¿y la interna?");
        externa.Unwrap().Wait();      // ahora sí esperamos por la interna

        // 6) Excepciones y estado
        Task falla = Task.Run(() =>
        {
            throw new InvalidOperationException("algo salió mal");
        });
        try
        {
            falla.Wait();
        }
        catch (AggregateException ex)
        {
            Console.WriteLine("Excepción: {0}", ex.InnerException.Message);
        }
        Console.WriteLine("Estado de la tarea: {0}", falla.Status);   // Faulted
    }
}
