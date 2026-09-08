namespace Common.Dominio;

/// <summary>
/// Entrada del historial de actividades de un usuario (CR10):
/// inicio de sesión, vehículos creados/eliminados/modificados y reservas hechas.
/// (Sin "compras": ese requerimiento se saca de la letra, va en el Obligatorio 3.)
/// </summary>
public class ActividadLog
{
    public string Usuario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
