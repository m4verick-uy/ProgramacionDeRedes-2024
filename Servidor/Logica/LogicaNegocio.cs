using Common.Dominio;
using Servidor.Almacenamiento;


namespace Servidor.Logica;

/// <summary>
/// Reglas de negocio del sistema Edison. Cada método corresponde (más o menos)
/// a un requerimiento de la letra. Trabaja sobre el Almacen en memoria.
///
/// Errores de negocio se comunican lanzando ExcepcionNegocio, con un mensaje
/// legible que el servidor devuelve al cliente como RespuestaError.
/// </summary>
public class LogicaNegocio
{
    private readonly Almacen _almacen;

    public LogicaNegocio(Almacen almacen)
    {
        _almacen = almacen;
    }

    // ---------------- CR1: Autenticación ----------------

    public void Registrar(string usuario, string contrasena)
    {
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contrasena))
            throw new ExcepcionNegocio("Usuario y contraseña no pueden estar vacíos.");

        if (_almacen.ExisteUsuario(usuario))
            throw new ExcepcionNegocio("Ya existe un usuario con ese nombre.");

        _almacen.AgregarUsuario(new Usuario { NombreUsuario = usuario, Contrasena = contrasena });
        Console.WriteLine($"[DEBUG] Registrado: user='{usuario}' (len {usuario.Length}) pass='{contrasena}' (len {contrasena.Length})");
    }

    public void Login(string usuario, string contrasena)
    {

        var u = _almacen.ObtenerUsuario(usuario);
        Console.WriteLine($"[DEBUG] Login: user='{usuario}' (len {usuario.Length}) pass='{contrasena}' (len {contrasena.Length}) | encontrado={u != null}" + (u != null ? $" passGuardada='{u.Contrasena}' (len {u.Contrasena.Length})" : ""));
        if (u == null || u.Contrasena != contrasena)
            throw new ExcepcionNegocio("Usuario o contraseña incorrectos.");

        _almacen.RegistrarActividad(usuario, "Inicio de sesión");

    }

    // ---------------- CR2: Alta de modelo ----------------

    public Modelo AltaModelo(string usuario, string nombre, string descripcion,
                             decimal precio, DateTime fechaEntrega, int stockInicial)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionNegocio("El nombre del modelo es obligatorio.");

        if (stockInicial < 0)
            throw new ExcepcionNegocio("El stock inicial no puede ser negativo.");

        if (_almacen.ExisteNombreModelo(nombre))
            throw new ExcepcionNegocio("Ya existe un modelo con ese nombre.");

        var modelo = _almacen.AgregarModelo(new Modelo
        {
            Nombre = nombre,
            Descripcion = descripcion,
            Precio = precio,
            FechaEntrega = fechaEntrega,
            StockDisponible = stockInicial,
            CreadoPor = usuario
        });

        _almacen.RegistrarActividad(usuario, $"Creó el modelo '{nombre}' (Id {modelo.Id})");
        return modelo;
    }

    // ---------------- CR3: Modificación de modelo ----------------

    public void ModificarModelo(string usuario, int modeloId, string descripcion,
                                decimal precio, DateTime fechaEntrega, int nuevoStock)
    {
        // Se toma el candado para que la validación de reservas y la escritura
        // sean atómicas respecto de otras operaciones (ej. reservas concurrentes).
        lock (_almacen.Candado)
        {
            var modelo = _almacen.ObtenerModelo(modeloId)
                ?? throw new ExcepcionNegocio("El modelo no existe.");

            if (modelo.CreadoPor != usuario)
                throw new ExcepcionNegocio("Solo el creador del modelo puede modificarlo.");

            // "Mientras sigan habiendo unidades disponibles" (letra).
            if (modelo.StockDisponible <= 0)
                throw new ExcepcionNegocio("No se puede modificar un modelo sin unidades disponibles.");

            if (nuevoStock < 0)
                throw new ExcepcionNegocio("El stock no puede ser negativo.");

            // El nuevo stock debe ser mayor a la cantidad de reservas existentes.
            // Contamos reservas activas = Pendiente + Confirmada (criterio del curso):
            // si alguien puso un stock menor a las pendientes, esas no tendrían
            // unidad y hay que evitarlo.
            int reservasActivas = _almacen.ObtenerReservasActivasDeModelo(modeloId).Count;
            if (nuevoStock <= reservasActivas)
                throw new ExcepcionNegocio(
                    $"El nuevo stock ({nuevoStock}) debe ser mayor a las reservas activas ({reservasActivas}).");

            modelo.Descripcion = descripcion;
            modelo.Precio = precio;
            modelo.FechaEntrega = fechaEntrega;
            modelo.StockDisponible = nuevoStock;

            _almacen.RegistrarActividad(usuario, $"Modificó el modelo '{modelo.Nombre}' (Id {modeloId})");
        }
    }

    // ---------------- CR4: Baja de modelo ----------------

    // Devuelve el nombre del archivo de imagen a borrar (o "" si no tenía),
    // para que el servidor elimine el archivo del disco (SR3).
    public string BajaModelo(string usuario, int modeloId)
    {
        lock (_almacen.Candado)
        {
            var modelo = _almacen.ObtenerModelo(modeloId)
                ?? throw new ExcepcionNegocio("El modelo no existe.");

            if (modelo.CreadoPor != usuario)
                throw new ExcepcionNegocio("Solo el creador del modelo puede eliminarlo.");

            // No debe haber reservas activas (Pendiente + Confirmada) asociadas.
            if (_almacen.ObtenerReservasActivasDeModelo(modeloId).Count > 0)
                throw new ExcepcionNegocio("No se puede eliminar: el modelo tiene reservas activas.");

            _almacen.EliminarModelo(modeloId);
            _almacen.RegistrarActividad(usuario, $"Eliminó el modelo '{modelo.Nombre}' (Id {modeloId})");

            return modelo.NombreArchivoImagen;
        }
    }

    // ---------------- CR2/CR8: Imagen ----------------

    public Modelo AsignarImagen(string usuario, int modeloId, string nombreArchivo)
    {
        lock (_almacen.Candado)
        {
            var modelo = _almacen.ObtenerModelo(modeloId)
                ?? throw new ExcepcionNegocio("El modelo no existe.");

            if (modelo.CreadoPor != usuario)
                throw new ExcepcionNegocio("Solo el creador del modelo puede subir su imagen.");

            modelo.NombreArchivoImagen = nombreArchivo;
            return modelo;
        }
    }

    // ---------------- CR7: Listado y filtrado ----------------

    // Filtros opcionales: si un parámetro es null, no se aplica ese filtro.
    public List<Modelo> ListarModelos(string? nombre, decimal? precioMin,
                                      decimal? precioMax, int? stockMin)
    {
        var modelos = _almacen.ObtenerModelos().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(nombre))
            modelos = modelos.Where(m => m.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

        if (precioMin.HasValue)
            modelos = modelos.Where(m => m.Precio >= precioMin.Value);

        if (precioMax.HasValue)
            modelos = modelos.Where(m => m.Precio <= precioMax.Value);

        if (stockMin.HasValue)
            modelos = modelos.Where(m => m.StockDisponible >= stockMin.Value);

        return modelos.OrderBy(m => m.Id).ToList();
    }

    // ---------------- CR8: Consultar modelo ----------------

    public Modelo ConsultarModelo(int modeloId)
    {
        return _almacen.ObtenerModelo(modeloId)
            ?? throw new ExcepcionNegocio("El modelo no existe.");
    }

    // Cantidad de unidades reservadas (reservas confirmadas) de un modelo.
    public int UnidadesReservadas(int modeloId)
    {
        return _almacen.ObtenerReservasActivasDeModelo(modeloId)
            .Count(r => r.Estado == EstadoReserva.Confirmada);
    }

    // ---------------- CR5: Solicitud de reserva ----------------

    // Crea la reserva en estado Pendiente y la devuelve. La resolución
    // (Confirmada/Rechazada) la hace el servidor luego de N segundos.
    public Reserva SolicitarReserva(string usuario, int modeloId)
    {
        lock (_almacen.Candado)
        {
            var modelo = _almacen.ObtenerModelo(modeloId)
                ?? throw new ExcepcionNegocio("El modelo no existe.");

            var reserva = _almacen.AgregarReserva(new Reserva
            {
                ModeloId = modeloId,
                NombreModelo = modelo.Nombre,
                Usuario = usuario,
                Estado = EstadoReserva.Pendiente,
                FechaCreacion = DateTime.Now,
                UltimaActualizacion = DateTime.Now
            });

            _almacen.RegistrarActividad(usuario,
                $"Solicitó reserva del modelo '{modelo.Nombre}' (Reserva {reserva.Id})");

            return reserva;
        }
    }

    // Resolución diferida de la reserva (la ejecuta un Thread tras N segundos).
    // Si hay stock -> Confirmada y descuenta; si no -> Rechazada.
    public void ProcesarReserva(int reservaId)
    {
        lock (_almacen.Candado)
        {
            var reserva = _almacen.ObtenerReserva(reservaId);
            if (reserva == null || reserva.Estado != EstadoReserva.Pendiente)
                return; // ya fue cancelada o procesada

            var modelo = _almacen.ObtenerModelo(reserva.ModeloId);

            if (modelo != null && modelo.StockDisponible > 0)
            {
                modelo.StockDisponible--;
                reserva.Estado = EstadoReserva.Confirmada;
            }
            else
            {
                reserva.Estado = EstadoReserva.Rechazada;
            }

            reserva.UltimaActualizacion = DateTime.Now;
        }
    }

    // ---------------- CR6: Cancelación de reserva ----------------

    public void CancelarReserva(string usuario, int reservaId)
    {
        lock (_almacen.Candado)
        {
            var reserva = _almacen.ObtenerReserva(reservaId)
                ?? throw new ExcepcionNegocio("La reserva no existe.");

            if (reserva.Usuario != usuario)
                throw new ExcepcionNegocio("Solo el dueño de la reserva puede cancelarla.");

            // Se puede cancelar siempre y cuando NO esté rechazada.
            if (reserva.Estado == EstadoReserva.Rechazada)
                throw new ExcepcionNegocio("No se puede cancelar una reserva rechazada.");

            if (reserva.Estado == EstadoReserva.Cancelada)
                throw new ExcepcionNegocio("La reserva ya estaba cancelada.");

            // Si estaba confirmada, la unidad vuelve a estar disponible.
            if (reserva.Estado == EstadoReserva.Confirmada)
            {
                var modelo = _almacen.ObtenerModelo(reserva.ModeloId);
                if (modelo != null)
                    modelo.StockDisponible++;
            }

            reserva.Estado = EstadoReserva.Cancelada;
            reserva.UltimaActualizacion = DateTime.Now;

            _almacen.RegistrarActividad(usuario, $"Canceló la reserva {reservaId}");
        }
    }

    // ---------------- CR9: Consultar reservas ----------------

    public List<Reserva> ListarReservas(string usuario)
        => _almacen.ObtenerReservasDeUsuario(usuario);

    // ---------------- CR10: Historial ----------------

    public List<ActividadLog> Historial(string usuario)
        => _almacen.ObtenerActividades(usuario);
}
