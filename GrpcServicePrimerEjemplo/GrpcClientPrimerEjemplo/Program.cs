using Grpc.Net.Client;
using GrpcClientPrimerEjemplo;

using var canal = GrpcChannel.ForAddress("http://localhost:5014");

var cliente = new Calculadora.CalculadoraClient(canal);

var respuesta = await cliente.SumarAsync(new SumaRequest { A = 15, B = 27 });

Console.WriteLine($"[CLIENT] Resultado: {respuesta.Resultado}");

