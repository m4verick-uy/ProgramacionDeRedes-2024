namespace Common.Dominio;

/// <summary>
/// Estados de una reserva (CR5, CR6).
///   Pendiente  -> recién solicitada, aún no procesada.
///   Confirmada -> había stock al procesarla; descontó una unidad.
///   Rechazada  -> no había stock al procesarla.
///   Cancelada  -> el usuario la canceló (solo si no estaba Rechazada).
///
/// Reservas ACTIVAS = Pendiente + Confirmada (criterio del curso).
/// Se usan para: validar baja de modelo (CR4) y validar stock al modificar (CR3).
/// </summary>
public enum EstadoReserva
{
    Pendiente = 0,
    Confirmada = 1,
    Rechazada = 2,
    Cancelada = 3
}

/// <summary>
/// Reserva de una unidad de un modelo, hecha por un usuario (CR5, CR6, CR9).
/// </summary>
public class Reserva
{
    public int Id { get; set; }
    public int ModeloId { get; set; }
    public string NombreModelo { get; set; } = string.Empty; // copia para mostrar en CR9
    public string Usuario { get; set; } = string.Empty;      // quién reservó
    public EstadoReserva Estado { get; set; } = EstadoReserva.Pendiente;
    public DateTime FechaCreacion { get; set; }
    public DateTime UltimaActualizacion { get; set; }

    public bool EsActiva =>
        Estado == EstadoReserva.Pendiente || Estado == EstadoReserva.Confirmada;
}
