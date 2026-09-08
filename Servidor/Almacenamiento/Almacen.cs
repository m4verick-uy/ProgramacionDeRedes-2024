using Common.Dominio;

namespace Servidor.Almacenamiento;

/// <summary>
/// "Base de datos" en memoria del servidor. Es lo más simple posible:
/// diccionarios y listas en RAM. Al reiniciar el servidor, los datos se pierden.
///
/// Todos los accesos se protegen con un único candado (_candado). Como el
/// servidor atiende cada cliente en un Thread distinto (SR2), varios hilos
/// pueden tocar estas colecciones a la vez, y sin lock habría condiciones de
/// carrera (ej. dos reservas confirmando la última unidad de stock).
///
/// La estrategia es simple y fácil de defender: un lock global para toda la
/// data. No es lo más performante, pero para el alcance del obligatorio es
/// correcto y claro.
/// </summary>
public class Almacen
{
    // Un único candado para toda la data (simple y suficiente para el obligatorio).
    private readonly object _candado = new();

    // Colecciones en memoria.
    private readonly Dictionary<string, Usuario> _usuarios = new(); // clave: NombreUsuario
    private readonly Dictionary<int, Modelo> _modelos = new();      // clave: Id
    private readonly Dictionary<int, Reserva> _reservas = new();    // clave: Id
    private readonly List<ActividadLog> _actividades = new();

    // Generadores de Id incremental.
    private int _proximoIdModelo = 1;
    private int _proximoIdReserva = 1;

    // Se expone el candado para operaciones compuestas que deben ser atómicas
    // (ej. "verificar stock Y confirmar reserva" en un solo bloque).
    public object Candado => _candado;

    // ---------------- Usuarios (CR1) ----------------

    public bool ExisteUsuario(string nombre)
    {
        lock (_candado) { return _usuarios.ContainsKey(nombre); }
    }

    public void AgregarUsuario(Usuario usuario)
    {
        lock (_candado) { _usuarios[usuario.NombreUsuario] = usuario; }
    }

    public Usuario? ObtenerUsuario(string nombre)
    {
        lock (_candado)
        {
            return _usuarios.TryGetValue(nombre, out var u) ? u : null;
        }
    }

    // ---------------- Modelos (CR2, CR3, CR4, CR7, CR8) ----------------

    public Modelo AgregarModelo(Modelo modelo)
    {
        lock (_candado)
        {
            modelo.Id = _proximoIdModelo++;
            _modelos[modelo.Id] = modelo;
            return modelo;
        }
    }

    public bool ExisteNombreModelo(string nombre)
    {
        lock (_candado)
        {
            return _modelos.Values.Any(m =>
                m.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        }
    }

    public Modelo? ObtenerModelo(int id)
    {
        lock (_candado)
        {
            return _modelos.TryGetValue(id, out var m) ? m : null;
        }
    }

    public List<Modelo> ObtenerModelos()
    {
        lock (_candado) { return _modelos.Values.ToList(); }
    }

    public void EliminarModelo(int id)
    {
        lock (_candado) { _modelos.Remove(id); }
    }

    // ---------------- Reservas (CR5, CR6, CR9) ----------------

    public Reserva AgregarReserva(Reserva reserva)
    {
        lock (_candado)
        {
            reserva.Id = _proximoIdReserva++;
            _reservas[reserva.Id] = reserva;
            return reserva;
        }
    }

    public Reserva? ObtenerReserva(int id)
    {
        lock (_candado)
        {
            return _reservas.TryGetValue(id, out var r) ? r : null;
        }
    }

    public List<Reserva> ObtenerReservasDeUsuario(string usuario)
    {
        lock (_candado)
        {
            return _reservas.Values
                .Where(r => r.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    // Reservas ACTIVAS (Pendiente + Confirmada) de un modelo. Base de CR3 y CR4.
    public List<Reserva> ObtenerReservasActivasDeModelo(int modeloId)
    {
        lock (_candado)
        {
            return _reservas.Values
                .Where(r => r.ModeloId == modeloId && r.EsActiva)
                .ToList();
        }
    }

    // ---------------- Historial (CR10) ----------------

    public void RegistrarActividad(string usuario, string descripcion)
    {
        lock (_candado)
        {
            _actividades.Add(new ActividadLog
            {
                Usuario = usuario,
                Fecha = DateTime.Now,
                Descripcion = descripcion
            });
        }
    }

    public List<ActividadLog> ObtenerActividades(string usuario)
    {
        lock (_candado)
        {
            return _actividades
                .Where(a => a.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Fecha)
                .ToList();
        }
    }
}
