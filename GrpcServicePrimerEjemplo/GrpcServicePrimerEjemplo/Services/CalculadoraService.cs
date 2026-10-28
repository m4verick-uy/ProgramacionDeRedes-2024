using Grpc.Core;

namespace GrpcServicePrimerEjemplo.Services;

public class CalculadoraService : Calculadora.CalculadoraBase
//                                 ↑ generada por protoc a partir del .proto
//                                   equivale a : ICalculadora
{
    public override Task<SumaReply> Sumar(SumaRequest request, ServerCallContext context)
    {
        double resultado = request.A + request.B;
        Console.WriteLine($"[SERVER] {request.A} + {request.B} = {resultado}");
        return Task.FromResult(new SumaReply { Resultado = resultado });
    }
}