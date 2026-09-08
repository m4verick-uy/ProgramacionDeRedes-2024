using System.Configuration;

namespace Servidor;

/// <summary>
/// Configuración del servidor leída desde App.config (portable sin recompilar).
/// Usamos System.Configuration.ConfigurationManager para leer los valores.
/// </summary>
public static class Configuracion
{
    public static string Ip =>
        ConfigurationManager.AppSettings["ServidorIp"] ?? "127.0.0.1";

    public static int Puerto =>
        int.Parse(ConfigurationManager.AppSettings["ServidorPuerto"] ?? "20000");

    // Backlog de la cola de conexiones (parámetro de Socket.Listen).
    public static int Backlog =>
        int.Parse(ConfigurationManager.AppSettings["Backlog"] ?? "100");

    // CR5: tiempo (en segundos) que una reserva queda Pendiente antes de
    // resolverse a Confirmada o Rechazada.
    public static int SegundosProcesarReserva =>
        int.Parse(ConfigurationManager.AppSettings["SegundosProcesarReserva"] ?? "5");

    // Carpeta donde el servidor guarda las imágenes de los modelos.
    public static string CarpetaImagenes =>
        ConfigurationManager.AppSettings["CarpetaImagenes"] ?? "imagenes";
}
