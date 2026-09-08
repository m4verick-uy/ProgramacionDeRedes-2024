namespace Common.Protocolo;

/// <summary>
/// Catálogo de comandos del protocolo. Cada operación de la letra tiene un
/// número (0-99) que viaja en el campo CMD de la trama.
///
/// Convención de rangos (solo para lectura humana, no impone lógica):
///   01-49  peticiones del cliente (request)
///   50-99  respuestas del servidor (response)
///
/// El cliente siempre manda un request y el servidor siempre contesta con el
/// response correspondiente sobre el mismo socket, así que en la práctica el
/// sentido queda determinado por quién envía.
/// </summary>
public enum Comando
{
    // -------- Peticiones (Cliente -> Servidor) --------
    Registro = 1,          // CR1
    Login = 2,             // CR1
    Logout = 3,            // CR1
    AltaModelo = 10,       // CR2
    SubirImagen = 11,      // imagen por stream (comando aparte, ver decisión de diseño)
    ModificarModelo = 12,  // CR3
    BajaModelo = 13,       // CR4
    ListarModelos = 14,    // CR7 (listado + filtros)
    ConsultarModelo = 15,  // CR8
    DescargarImagen = 16,  // CR8 (opcional: bajar la imagen)
    SolicitarReserva = 20, // CR5
    CancelarReserva = 21,  // CR6
    ListarReservas = 22,   // CR9
    Historial = 23,        // CR10

    // -------- Respuestas (Servidor -> Cliente) --------
    RespuestaOk = 50,      // operación exitosa (DATOS = payload de la respuesta)
    RespuestaError = 51    // operación fallida (DATOS = mensaje de error legible)
}
