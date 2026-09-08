using System.Configuration;

namespace Cliente;

/// <summary>
/// Configuración del cliente leída desde App.config (portable sin recompilar).
/// IP y puerto del SERVIDOR al que se conecta.
/// </summary>
public static class Configuracion
{
    public static string ServidorIp =>
        ConfigurationManager.AppSettings["ServidorIp"] ?? "127.0.0.1";

    public static int ServidorPuerto =>
        int.Parse(ConfigurationManager.AppSettings["ServidorPuerto"] ?? "20000");
}
