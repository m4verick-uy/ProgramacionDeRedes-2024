namespace Servidor.Logica;

/// <summary>
/// Error de negocio esperable (ej. "usuario ya existe", "sin stock").
/// El servidor la captura y devuelve su mensaje al cliente como RespuestaError,
/// sin caerse (manejo de errores: el servidor sigue funcionando).
/// </summary>
public class ExcepcionNegocio : Exception
{
    public ExcepcionNegocio(string mensaje) : base(mensaje) { }
}
