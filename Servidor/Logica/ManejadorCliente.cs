using System.Net.Sockets;
using Common.Dominio;
using Common.Protocolo;
using Common.Red;
using Servidor.Almacenamiento;

namespace Servidor.Logica;

// Nota: Configuracion vive en el namespace 'Servidor' (contenedor de este
// 'Servidor.Logica'), por lo que queda en scope sin necesidad de un using.

/// <summary>
/// Atiende a UN cliente en su propio Thread (SR2). Lee comandos en un bucle,
/// los procesa contra la lógica de negocio y devuelve una respuesta.
///
/// Manejo de desconexión: cuando el cliente cierra, Receive devuelve 0 y el
/// NetworkDataHelper lanza SocketException. La capturamos para cortar el bucle
/// limpiamente y liberar el socket, sin que el servidor se cuelgue ni se caiga.
/// </summary>
public class ManejadorCliente
{
    private readonly Socket _socket;
    private readonly NetworkDataHelper _red;
    private readonly LogicaNegocio _logica;
    private readonly Almacen _almacen;
    private readonly ServidorTcp _servidor;

    // Usuario autenticado en esta conexión (null = todavía no hizo login).
    private string? _usuario;

    public ManejadorCliente(Socket socket, LogicaNegocio logica, Almacen almacen, ServidorTcp servidor)
    {
        _socket = socket;
        _red = new NetworkDataHelper(socket);
        _logica = logica;
        _almacen = almacen;
        _servidor = servidor;
    }

    public void Atender()
    {
        bool conectado = true;
        try
        {
            while (conectado && _servidor.EnEjecucion)
            {
                var (comando, datos) = _red.RecibirMensaje();

                if (comando == Comando.Logout)
                {
                    conectado = false;
                    continue;
                }

                Despachar(comando, datos);
            }
        }
        catch (SocketException)
        {
            // El cliente se desconectó (Receive devolvió 0) o hubo un corte de red.
            // Es un caso esperado: solo salimos del bucle.
        }
        catch (ObjectDisposedException)
        {
            // El socket fue cerrado por el cierre controlado del servidor (SR4)
            // mientras este handler estaba bloqueado en Receive. Es esperado.
        }
        catch (Exception ex)
        {
            // Cualquier otro error inesperado: lo registramos pero no tumbamos el servidor.
            Console.WriteLine($"[Servidor] Error atendiendo cliente: {ex.Message}");
        }
        finally
        {
            _servidor.DesregistrarCliente(_socket);
            CerrarSocket();
            Console.WriteLine("[Servidor] Cliente desconectado.");
        }
    }

    // Enruta cada comando a su handler. Los errores de negocio se devuelven
    // como RespuestaError; el servidor nunca se cae por un pedido malo.
    private void Despachar(Comando comando, byte[] datos)
    {
        try
        {
            switch (comando)
            {
                case Comando.Registro:        Registro(datos); break;
                case Comando.Login:           Login(datos); break;
                case Comando.AltaModelo:      AltaModelo(datos); break;
                case Comando.SubirImagen:     SubirImagen(datos); break;
                case Comando.ModificarModelo: ModificarModelo(datos); break;
                case Comando.BajaModelo:      BajaModelo(datos); break;
                case Comando.ListarModelos:   ListarModelos(datos); break;
                case Comando.ConsultarModelo: ConsultarModelo(datos); break;
                case Comando.DescargarImagen: DescargarImagen(datos); break;
                case Comando.SolicitarReserva:SolicitarReserva(datos); break;
                case Comando.CancelarReserva: CancelarReserva(datos); break;
                case Comando.ListarReservas:  ListarReservas(); break;
                case Comando.Historial:       Historial(); break;
                default:
                    ResponderError("Comando no reconocido.");
                    break;
            }
        }
        catch (ExcepcionNegocio ex)
        {
            ResponderError(ex.Message);
        }
    }

    // Exige que el cliente esté autenticado antes de operar.
    private string ExigirLogin()
    {
        if (_usuario == null)
            throw new ExcepcionNegocio("Debe iniciar sesión primero.");
        return _usuario;
    }

    // ---------------- Handlers ----------------

    private void Registro(byte[] datos)
    {
        var p = new Paquete(datos);
        string usuario = p.LeerString();
        string contrasena = p.LeerString();

        _logica.Registrar(usuario, contrasena);
        ResponderOk("Usuario registrado con éxito.");
    }

    private void Login(byte[] datos)
    {
        var p = new Paquete(datos);
        string usuario = p.LeerString();
        string contrasena = p.LeerString();

        _logica.Login(usuario, contrasena);
        _usuario = usuario;
        ResponderOk("Inicio de sesión correcto.");
    }

    private void AltaModelo(byte[] datos)
    {
        string usuario = ExigirLogin();
        var p = new Paquete(datos);
        string nombre = p.LeerString();
        string descripcion = p.LeerString();
        decimal precio = p.LeerDecimal();
        DateTime fechaEntrega = p.LeerDateTime();
        int stock = p.LeerInt();

        var modelo = _logica.AltaModelo(usuario, nombre, descripcion, precio, fechaEntrega, stock);

        var resp = new Paquete();
        resp.EscribirInt(modelo.Id);
        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    // Imagen POR STREAM: recibimos idModelo + los bytes crudos de la imagen.
    // Los bytes de la imagen NO se convierten a texto: viajan como campo binario
    // (largo + bytes) dentro del Paquete, y se escriben tal cual a disco.
    private void SubirImagen(byte[] datos)
    {
        string usuario = ExigirLogin();
        var p = new Paquete(datos);
        int modeloId = p.LeerInt();
        string extension = p.LeerString();     // ej. ".jpg"
        byte[] imagen = p.LeerBytes();         // bytes crudos de la imagen

        string nombreArchivo = $"modelo_{modeloId}{extension}";
        string ruta = Path.Combine(Configuracion.CarpetaImagenes, nombreArchivo);

        Directory.CreateDirectory(Configuracion.CarpetaImagenes);
        File.WriteAllBytes(ruta, imagen);

        _logica.AsignarImagen(usuario, modeloId, nombreArchivo);
        ResponderOk("Imagen subida con éxito.");
    }

    private void ModificarModelo(byte[] datos)
    {
        string usuario = ExigirLogin();
        var p = new Paquete(datos);
        int modeloId = p.LeerInt();
        string descripcion = p.LeerString();
        decimal precio = p.LeerDecimal();
        DateTime fechaEntrega = p.LeerDateTime();
        int nuevoStock = p.LeerInt();

        _logica.ModificarModelo(usuario, modeloId, descripcion, precio, fechaEntrega, nuevoStock);
        ResponderOk("Modelo modificado con éxito.");
    }

    private void BajaModelo(byte[] datos)
    {
        string usuario = ExigirLogin();
        var p = new Paquete(datos);
        int modeloId = p.LeerInt();

        // La lógica devuelve el nombre del archivo de imagen a borrar (SR3).
        string archivoImagen = _logica.BajaModelo(usuario, modeloId);
        BorrarImagenSiExiste(archivoImagen);

        ResponderOk("Modelo eliminado con éxito.");
    }

    private void ListarModelos(byte[] datos)
    {
        var p = new Paquete(datos);
        // Filtros: mandamos cada uno como string; "" significa "no filtrar".
        string nombre = p.LeerString();
        string precioMinTxt = p.LeerString();
        string precioMaxTxt = p.LeerString();
        string stockMinTxt = p.LeerString();

        decimal? precioMin = ParsearDecimalOpcional(precioMinTxt);
        decimal? precioMax = ParsearDecimalOpcional(precioMaxTxt);
        int? stockMin = ParsearIntOpcional(stockMinTxt);

        var modelos = _logica.ListarModelos(
            string.IsNullOrWhiteSpace(nombre) ? null : nombre,
            precioMin, precioMax, stockMin);

        var resp = new Paquete();
        resp.EscribirInt(modelos.Count);
        foreach (var m in modelos)
            EscribirModelo(resp, m);

        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    private void ConsultarModelo(byte[] datos)
    {
        var p = new Paquete(datos);
        int modeloId = p.LeerInt();

        var modelo = _logica.ConsultarModelo(modeloId);
        int reservadas = _logica.UnidadesReservadas(modeloId);

        var resp = new Paquete();
        EscribirModelo(resp, modelo);
        resp.EscribirInt(reservadas);
        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    // Descarga de imagen (CR8, opcional): devolvemos los bytes crudos.
    private void DescargarImagen(byte[] datos)
    {
        var p = new Paquete(datos);
        int modeloId = p.LeerInt();

        var modelo = _logica.ConsultarModelo(modeloId);
        if (!modelo.TieneImagen)
            throw new ExcepcionNegocio("El modelo no tiene imagen.");

        string ruta = Path.Combine(Configuracion.CarpetaImagenes, modelo.NombreArchivoImagen);
        if (!File.Exists(ruta))
            throw new ExcepcionNegocio("La imagen no está disponible en el servidor.");

        byte[] imagen = File.ReadAllBytes(ruta);
        string extension = Path.GetExtension(modelo.NombreArchivoImagen);

        var resp = new Paquete();
        resp.EscribirString(extension);
        resp.EscribirBytes(imagen);   // bytes crudos, sin pasar por texto
        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    private void SolicitarReserva(byte[] datos)
    {
        string usuario = ExigirLogin();
        var p = new Paquete(datos);
        int modeloId = p.LeerInt();

        var reserva = _logica.SolicitarReserva(usuario, modeloId);

        // CR5: la reserva se resuelve luego de N segundos, en un Thread aparte,
        // para no bloquear la atención de este cliente.
        _servidor.ProgramarProcesoReserva(reserva.Id);

        var resp = new Paquete();
        resp.EscribirInt(reserva.Id);
        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    private void CancelarReserva(byte[] datos)
    {
        string usuario = ExigirLogin();
        var p = new Paquete(datos);
        int reservaId = p.LeerInt();

        _logica.CancelarReserva(usuario, reservaId);
        ResponderOk("Reserva cancelada con éxito.");
    }

    private void ListarReservas()
    {
        string usuario = ExigirLogin();
        var reservas = _logica.ListarReservas(usuario);

        var resp = new Paquete();
        resp.EscribirInt(reservas.Count);
        foreach (var r in reservas)
        {
            resp.EscribirInt(r.Id);
            resp.EscribirString(r.NombreModelo);
            resp.EscribirString(r.Estado.ToString());
            resp.EscribirDateTime(r.FechaCreacion);
            resp.EscribirDateTime(r.UltimaActualizacion);
        }
        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    private void Historial()
    {
        string usuario = ExigirLogin();
        var actividades = _logica.Historial(usuario);

        var resp = new Paquete();
        resp.EscribirInt(actividades.Count);
        foreach (var a in actividades)
        {
            resp.EscribirDateTime(a.Fecha);
            resp.EscribirString(a.Descripcion);
        }
        _red.EnviarMensaje(Comando.RespuestaOk, resp);
    }

    // ---------------- Helpers de respuesta ----------------

    private void ResponderOk(string mensaje)
    {
        var p = new Paquete();
        p.EscribirString(mensaje);
        _red.EnviarMensaje(Comando.RespuestaOk, p);
    }

    private void ResponderError(string mensaje)
    {
        var p = new Paquete();
        p.EscribirString(mensaje);
        _red.EnviarMensaje(Comando.RespuestaError, p);
    }

    // Serializa un modelo dentro de un paquete (orden fijo, contrato con el cliente).
    private static void EscribirModelo(Paquete p, Modelo m)
    {
        p.EscribirInt(m.Id);
        p.EscribirString(m.Nombre);
        p.EscribirString(m.Descripcion);
        p.EscribirDecimal(m.Precio);
        p.EscribirDateTime(m.FechaEntrega);
        p.EscribirInt(m.StockDisponible);
        p.EscribirString(m.CreadoPor);
        p.EscribirString(m.TieneImagen ? "SI" : "NO");
    }

    // ---------------- Utilitarios ----------------

    private static decimal? ParsearDecimalOpcional(string txt) =>
        string.IsNullOrWhiteSpace(txt)
            ? null
            : decimal.Parse(txt, System.Globalization.CultureInfo.InvariantCulture);

    private static int? ParsearIntOpcional(string txt) =>
        string.IsNullOrWhiteSpace(txt) ? null : int.Parse(txt);

    private static void BorrarImagenSiExiste(string nombreArchivo)
    {
        if (string.IsNullOrEmpty(nombreArchivo)) return;
        string ruta = Path.Combine(Configuracion.CarpetaImagenes, nombreArchivo);
        if (File.Exists(ruta))
            File.Delete(ruta);
    }

    private void CerrarSocket()
    {
        try
        {
            _socket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException)
        {
            // El socket ya podía estar cerrado por el otro extremo; lo ignoramos.
        }
        finally
        {
            _socket.Close();
        }
    }
}
