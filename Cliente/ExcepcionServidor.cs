namespace Cliente;

/// <summary>
/// El servidor respondió con RespuestaError. Contiene el mensaje legible para
/// mostrárselo al usuario en el menú.
/// </summary>
public class ExcepcionServidor : Exception
{
    public ExcepcionServidor(string mensaje) : base(mensaje) { }
}
