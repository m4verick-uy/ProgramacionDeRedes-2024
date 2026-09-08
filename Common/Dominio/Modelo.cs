namespace Common.Dominio;

/// <summary>
/// Modelo de vehículo del catálogo (CR2, CR3, CR4).
///
/// StockDisponible = unidades libres para reservar en este momento.
/// Al confirmarse una reserva, se descuenta del stock; al cancelarse una
/// reserva no rechazada, se devuelve.
///
/// La imagen NO viaja dentro de esta entidad: se transfiere aparte por el
/// socket (comando SubirImagen / DescargarImagen). Acá solo guardamos el
/// nombre del archivo de imagen en el servidor (o vacío si no tiene).
/// </summary>
public class Modelo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;         // único
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public DateTime FechaEntrega { get; set; }
    public int StockDisponible { get; set; }
    public string CreadoPor { get; set; } = string.Empty;      // NombreUsuario del creador
    public string NombreArchivoImagen { get; set; } = string.Empty; // "" si no tiene imagen

    public bool TieneImagen => !string.IsNullOrEmpty(NombreArchivoImagen);
}
