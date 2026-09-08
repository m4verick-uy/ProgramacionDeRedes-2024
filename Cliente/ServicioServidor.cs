using System.Net;
using System.Net.Sockets;
using Common.Dominio;
using Common.Protocolo;
using Common.Red;

namespace Cliente;

/// <summary>
/// Encapsula la conexión con el servidor y expone un método por operación.
/// Cada método arma el Paquete del request, lo envía y devuelve la respuesta ya
/// interpretada. Si el servidor responde RespuestaError, lanza ExcepcionServidor
/// con el mensaje, para que el menú lo muestre.
/// </summary>
public class ServicioServidor
{
    private readonly Socket _socket;
    private readonly NetworkDataHelper _red;

    public ServicioServidor()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Endpoint local con puerto 0: el sistema asigna cualquier puerto libre.
        var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
        var remoteEndpoint = new IPEndPoint(IPAddress.Parse(Configuracion.ServidorIp), Configuracion.ServidorPuerto);

        _socket.Bind(localEndpoint);
        _socket.Connect(remoteEndpoint);

        _red = new NetworkDataHelper(_socket);
    }

    // Cierre controlado del cliente: avisa logout, luego shutdown + close.
    public void Desconectar()
    {
        try
        {
            _red.EnviarMensaje(Comando.Logout, Array.Empty<byte>());
            _socket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException)
        {
            // El servidor pudo haber cerrado ya; lo ignoramos.
        }
        finally
        {
            _socket.Close();
        }
    }

    // ---------------- CR1 ----------------

    public string Registrar(string usuario, string contrasena)
    {
        var p = new Paquete();
        p.EscribirString(usuario);
        p.EscribirString(contrasena);
        _red.EnviarMensaje(Comando.Registro, p);
        return LeerRespuestaTexto();
    }

    public string Login(string usuario, string contrasena)
    {
        var p = new Paquete();
        p.EscribirString(usuario);
        p.EscribirString(contrasena);
        _red.EnviarMensaje(Comando.Login, p);
        return LeerRespuestaTexto();
    }

    // ---------------- CR2 ----------------

    public int AltaModelo(string nombre, string descripcion, decimal precio,
                          DateTime fechaEntrega, int stock)
    {
        var p = new Paquete();
        p.EscribirString(nombre);
        p.EscribirString(descripcion);
        p.EscribirDecimal(precio);
        p.EscribirDateTime(fechaEntrega);
        p.EscribirInt(stock);
        _red.EnviarMensaje(Comando.AltaModelo, p);

        var resp = LeerRespuesta();
        return resp.LeerInt(); // Id del modelo creado
    }

    // Imagen por stream: enviamos id + extensión + bytes crudos del archivo.
    public string SubirImagen(int modeloId, string rutaArchivo)
    {
        byte[] imagen = File.ReadAllBytes(rutaArchivo);
        string extension = Path.GetExtension(rutaArchivo);

        var p = new Paquete();
        p.EscribirInt(modeloId);
        p.EscribirString(extension);
        p.EscribirBytes(imagen);
        _red.EnviarMensaje(Comando.SubirImagen, p);
        return LeerRespuestaTexto();
    }

    // ---------------- CR3 ----------------

    public string ModificarModelo(int modeloId, string descripcion, decimal precio,
                                  DateTime fechaEntrega, int nuevoStock)
    {
        var p = new Paquete();
        p.EscribirInt(modeloId);
        p.EscribirString(descripcion);
        p.EscribirDecimal(precio);
        p.EscribirDateTime(fechaEntrega);
        p.EscribirInt(nuevoStock);
        _red.EnviarMensaje(Comando.ModificarModelo, p);
        return LeerRespuestaTexto();
    }

    // ---------------- CR4 ----------------

    public string BajaModelo(int modeloId)
    {
        var p = new Paquete();
        p.EscribirInt(modeloId);
        _red.EnviarMensaje(Comando.BajaModelo, p);
        return LeerRespuestaTexto();
    }

    // ---------------- CR7 ----------------

    // Filtros opcionales: pasar "" (o 0 para stock) para no aplicar.
    public List<Modelo> ListarModelos(string nombre, string precioMin,
                                      string precioMax, string stockMin)
    {
        var p = new Paquete();
        p.EscribirString(nombre);
        p.EscribirString(precioMin);
        p.EscribirString(precioMax);
        p.EscribirString(stockMin);
        _red.EnviarMensaje(Comando.ListarModelos, p);

        var resp = LeerRespuesta();
        int cantidad = resp.LeerInt();
        var modelos = new List<Modelo>();
        for (int i = 0; i < cantidad; i++)
            modelos.Add(LeerModelo(resp));
        return modelos;
    }

    // ---------------- CR8 ----------------

    public (Modelo modelo, int reservadas) ConsultarModelo(int modeloId)
    {
        var p = new Paquete();
        p.EscribirInt(modeloId);
        _red.EnviarMensaje(Comando.ConsultarModelo, p);

        var resp = LeerRespuesta();
        var modelo = LeerModelo(resp);
        int reservadas = resp.LeerInt();
        return (modelo, reservadas);
    }

    public string DescargarImagen(int modeloId, string carpetaDestino)
    {
        var p = new Paquete();
        p.EscribirInt(modeloId);
        _red.EnviarMensaje(Comando.DescargarImagen, p);

        var resp = LeerRespuesta();
        string extension = resp.LeerString();
        byte[] imagen = resp.LeerBytes();

        Directory.CreateDirectory(carpetaDestino);
        string ruta = Path.Combine(carpetaDestino, $"modelo_{modeloId}{extension}");
        File.WriteAllBytes(ruta, imagen);
        return ruta;
    }

    // ---------------- CR5 ----------------

    public int SolicitarReserva(int modeloId)
    {
        var p = new Paquete();
        p.EscribirInt(modeloId);
        _red.EnviarMensaje(Comando.SolicitarReserva, p);

        var resp = LeerRespuesta();
        return resp.LeerInt(); // Id de la reserva
    }

    // ---------------- CR6 ----------------

    public string CancelarReserva(int reservaId)
    {
        var p = new Paquete();
        p.EscribirInt(reservaId);
        _red.EnviarMensaje(Comando.CancelarReserva, p);
        return LeerRespuestaTexto();
    }

    // ---------------- CR9 ----------------

    public List<Reserva> ListarReservas()
    {
        _red.EnviarMensaje(Comando.ListarReservas, Array.Empty<byte>());

        var resp = LeerRespuesta();
        int cantidad = resp.LeerInt();
        var reservas = new List<Reserva>();
        for (int i = 0; i < cantidad; i++)
        {
            var r = new Reserva
            {
                Id = resp.LeerInt(),
                NombreModelo = resp.LeerString(),
                Estado = Enum.Parse<EstadoReserva>(resp.LeerString()),
                FechaCreacion = resp.LeerDateTime(),
                UltimaActualizacion = resp.LeerDateTime()
            };
            reservas.Add(r);
        }
        return reservas;
    }

    // ---------------- CR10 ----------------

    public List<ActividadLog> Historial()
    {
        _red.EnviarMensaje(Comando.Historial, Array.Empty<byte>());

        var resp = LeerRespuesta();
        int cantidad = resp.LeerInt();
        var actividades = new List<ActividadLog>();
        for (int i = 0; i < cantidad; i++)
        {
            actividades.Add(new ActividadLog
            {
                Fecha = resp.LeerDateTime(),
                Descripcion = resp.LeerString()
            });
        }
        return actividades;
    }

    // ---------------- Helpers ----------------

    // Lee la respuesta del servidor. Si es error, lanza ExcepcionServidor.
    private Paquete LeerRespuesta()
    {
        var (comando, datos) = _red.RecibirMensaje();
        var p = new Paquete(datos);

        if (comando == Comando.RespuestaError)
            throw new ExcepcionServidor(p.LeerString());

        return p;
    }

    // Para respuestas cuyo payload es un único string (mensaje de OK).
    private string LeerRespuestaTexto() => LeerRespuesta().LeerString();

    // Deserializa un modelo con el MISMO orden con que lo escribe el servidor.
    private static Modelo LeerModelo(Paquete p)
    {
        var m = new Modelo
        {
            Id = p.LeerInt(),
            Nombre = p.LeerString(),
            Descripcion = p.LeerString(),
            Precio = p.LeerDecimal(),
            FechaEntrega = p.LeerDateTime(),
            StockDisponible = p.LeerInt(),
            CreadoPor = p.LeerString()
        };
        string tieneImagen = p.LeerString();
        m.NombreArchivoImagen = tieneImagen == "SI" ? "(en servidor)" : "";
        return m;
    }
}
