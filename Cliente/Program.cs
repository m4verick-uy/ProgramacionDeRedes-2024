using System.Globalization;
using Cliente;
using Common.Dominio;

Console.WriteLine("=== Cliente Edison ===");

ServicioServidor servidor;
try
{
    servidor = new ServicioServidor();
    Console.WriteLine($"Conectado al servidor {Configuracion.ServidorIp}:{Configuracion.ServidorPuerto}");
}
catch (Exception ex)
{
    Console.WriteLine($"No se pudo conectar al servidor: {ex.Message}");
    return;
}

try
{
    // Primero autenticación; luego el menú principal.
    if (MenuAutenticacion(servidor))
        MenuPrincipal(servidor);
}
catch (ExcepcionServidor ex)
{
    Console.WriteLine($"Error del servidor: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error inesperado: {ex.Message}");
}
finally
{
    servidor.Desconectar();
    Console.WriteLine("Cliente finalizado.");
}


// ================= Menús =================

// Devuelve true si el usuario quedó logueado; false si eligió salir.
static bool MenuAutenticacion(ServicioServidor servidor)
{
    bool autenticado = false;
    bool salir = false;

    while (!autenticado && !salir)
    {
        Console.WriteLine("\n--- Autenticación ---");
        Console.WriteLine("1) Registrarse");
        Console.WriteLine("2) Iniciar sesión");
        Console.WriteLine("0) Salir");
        Console.Write("Opción: ");
        string? opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1":
                {
                    string usuario = Pedir("Usuario");
                    string pass = Pedir("Contraseña");
                    Console.WriteLine(servidor.Registrar(usuario, pass));
                    break;
                }
                case "2":
                {
                    string usuario = Pedir("Usuario");
                    string pass = Pedir("Contraseña");
                    Console.WriteLine(servidor.Login(usuario, pass));
                    autenticado = true;
                    break;
                }
                case "0":
                    salir = true;
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }
        }
        catch (ExcepcionServidor ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    return autenticado;
}

static void MenuPrincipal(ServicioServidor servidor)
{
    bool salir = false;

    while (!salir)
    {
        Console.WriteLine("\n--- Menú principal ---");
        Console.WriteLine(" 1) Alta de modelo");
        Console.WriteLine(" 2) Subir imagen a un modelo");
        Console.WriteLine(" 3) Modificar modelo");
        Console.WriteLine(" 4) Eliminar modelo");
        Console.WriteLine(" 5) Listar / filtrar modelos");
        Console.WriteLine(" 6) Consultar modelo");
        Console.WriteLine(" 7) Descargar imagen de un modelo");
        Console.WriteLine(" 8) Solicitar reserva");
        Console.WriteLine(" 9) Cancelar reserva");
        Console.WriteLine("10) Ver mis reservas");
        Console.WriteLine("11) Ver mi historial");
        Console.WriteLine(" 0) Cerrar sesión y salir");
        Console.Write("Opción: ");
        string? opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1": AltaModelo(servidor); break;
                case "2": SubirImagen(servidor); break;
                case "3": ModificarModelo(servidor); break;
                case "4": EliminarModelo(servidor); break;
                case "5": ListarModelos(servidor); break;
                case "6": ConsultarModelo(servidor); break;
                case "7": DescargarImagen(servidor); break;
                case "8": SolicitarReserva(servidor); break;
                case "9": CancelarReserva(servidor); break;
                case "10": ListarReservas(servidor); break;
                case "11": Historial(servidor); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
        catch (ExcepcionServidor ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

// ================= Operaciones =================

static void AltaModelo(ServicioServidor servidor)
{
    string nombre = Pedir("Nombre del modelo");
    string descripcion = Pedir("Descripción");
    decimal precio = PedirDecimal("Precio");
    DateTime fecha = PedirFecha("Fecha de entrega (aaaa-mm-dd)");
    int stock = PedirInt("Stock inicial");

    int id = servidor.AltaModelo(nombre, descripcion, precio, fecha, stock);
    Console.WriteLine($"Modelo creado con Id {id}.");
}

static void SubirImagen(ServicioServidor servidor)
{
    int id = PedirInt("Id del modelo");
    string ruta = Pedir("Ruta del archivo de imagen");
    if (!File.Exists(ruta))
    {
        Console.WriteLine("El archivo no existe.");
        return;
    }
    Console.WriteLine(servidor.SubirImagen(id, ruta));
}

static void ModificarModelo(ServicioServidor servidor)
{
    int id = PedirInt("Id del modelo");
    string descripcion = Pedir("Nueva descripción");
    decimal precio = PedirDecimal("Nuevo precio");
    DateTime fecha = PedirFecha("Nueva fecha de entrega (aaaa-mm-dd)");
    int stock = PedirInt("Nuevo stock");

    Console.WriteLine(servidor.ModificarModelo(id, descripcion, precio, fecha, stock));
}

static void EliminarModelo(ServicioServidor servidor)
{
    int id = PedirInt("Id del modelo");
    Console.WriteLine(servidor.BajaModelo(id));
}

static void ListarModelos(ServicioServidor servidor)
{
    Console.WriteLine("Filtros (Enter para omitir cada uno):");
    string nombre = Pedir("Nombre contiene");
    string precioMin = Pedir("Precio mínimo");
    string precioMax = Pedir("Precio máximo");
    string stockMin = Pedir("Stock mínimo");

    var modelos = servidor.ListarModelos(nombre, precioMin, precioMax, stockMin);
    if (modelos.Count == 0)
    {
        Console.WriteLine("No hay modelos que coincidan.");
        return;
    }
    foreach (var m in modelos)
        Console.WriteLine($"[{m.Id}] {m.Nombre} | ${m.Precio} | stock: {m.StockDisponible} " +
                          $"| entrega: {m.FechaEntrega:yyyy-MM-dd} | imagen: {(string.IsNullOrEmpty(m.NombreArchivoImagen) ? "no" : "sí")}");
}

static void ConsultarModelo(ServicioServidor servidor)
{
    int id = PedirInt("Id del modelo");
    var (m, reservadas) = servidor.ConsultarModelo(id);
    Console.WriteLine($"Id: {m.Id}");
    Console.WriteLine($"Nombre: {m.Nombre}");
    Console.WriteLine($"Descripción: {m.Descripcion}");
    Console.WriteLine($"Precio: ${m.Precio}");
    Console.WriteLine($"Fecha de entrega: {m.FechaEntrega:yyyy-MM-dd}");
    Console.WriteLine($"Unidades disponibles: {m.StockDisponible}");
    Console.WriteLine($"Unidades reservadas: {reservadas}");
    Console.WriteLine($"Creado por: {m.CreadoPor}");
    Console.WriteLine($"Tiene imagen: {(string.IsNullOrEmpty(m.NombreArchivoImagen) ? "no" : "sí")}");
}

static void DescargarImagen(ServicioServidor servidor)
{
    int id = PedirInt("Id del modelo");
    string ruta = servidor.DescargarImagen(id, "descargas");
    Console.WriteLine($"Imagen guardada en: {ruta}");
}

static void SolicitarReserva(ServicioServidor servidor)
{
    int id = PedirInt("Id del modelo a reservar");
    int reservaId = servidor.SolicitarReserva(id);
    Console.WriteLine($"Reserva {reservaId} creada en estado Pendiente. " +
                      "Se resolverá (Confirmada/Rechazada) en unos segundos; " +
                      "consultá 'Ver mis reservas' para ver el resultado.");
}

static void CancelarReserva(ServicioServidor servidor)
{
    int id = PedirInt("Id de la reserva a cancelar");
    Console.WriteLine(servidor.CancelarReserva(id));
}

static void ListarReservas(ServicioServidor servidor)
{static void MenuPrincipal(ServicioServidor servidor)
{
    bool salir = false;

    while (!salir)
    {
        Console.WriteLine("\n--- Menú principal ---");
        Console.WriteLine(" 1) Alta de modelo");
        Console.WriteLine(" 2) Subir imagen a un modelo");
        Console.WriteLine(" 3) Modificar modelo");
        Console.WriteLine(" 4) Eliminar modelo");
        Console.WriteLine(" 5) Listar / filtrar modelos");
        Console.WriteLine(" 6) Consultar modelo");
        Console.WriteLine(" 7) Descargar imagen de un modelo");
        Console.WriteLine(" 8) Solicitar reserva");
        Console.WriteLine(" 9) Cancelar reserva");
        Console.WriteLine("10) Ver mis reservas");
        Console.WriteLine("11) Ver mi historial");
        Console.WriteLine(" 0) Cerrar sesión y salir");
        Console.Write("Opción: ");
        string? opcion = Console.ReadLine();

        try
        {
            switch (opcion)
            {
                case "1": AltaModelo(servidor); break;
                case "2": SubirImagen(servidor); break;
                case "3": ModificarModelo(servidor); break;
                case "4": EliminarModelo(servidor); break;
                case "5": ListarModelos(servidor); break;
                case "6": ConsultarModelo(servidor); break;
                case "7": DescargarImagen(servidor); break;
                case "8": SolicitarReserva(servidor); break;
                case "9": CancelarReserva(servidor); break;
                case "10": ListarReservas(servidor); break;
                case "11": Historial(servidor); break;
                case "0": salir = true; break;
                default: Console.WriteLine("Opción inválida."); break;
            }
        }
        catch (ExcepcionServidor ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
    var reservas = servidor.ListarReservas();
    if (reservas.Count == 0)
    {
        Console.WriteLine("No tenés reservas.");
        return;
    }
    foreach (var r in reservas)
        Console.WriteLine($"[{r.Id}] {r.NombreModelo} | estado: {r.Estado} " +
                          $"| creada: {r.FechaCreacion:yyyy-MM-dd HH:mm:ss} " +
                          $"| actualizada: {r.UltimaActualizacion:yyyy-MM-dd HH:mm:ss}");
}

static void Historial(ServicioServidor servidor)
{
    var actividades = servidor.Historial();
    if (actividades.Count == 0)
    {
        Console.WriteLine("Sin actividades registradas.");
        return;
    }
    foreach (var a in actividades)
        Console.WriteLine($"{a.Fecha:yyyy-MM-dd HH:mm:ss} - {a.Descripcion}");
}


// ================= Entrada de datos =================

static string Pedir(string etiqueta)
{
    Console.Write($"{etiqueta}: ");
    return Console.ReadLine() ?? string.Empty;
}

static int PedirInt(string etiqueta)
{
    int valor = 0;
    bool valido = false;
    while (!valido)
    {
        Console.Write($"{etiqueta}: ");
        valido = int.TryParse(Console.ReadLine(), out valor);
        if (!valido)
            Console.WriteLine("Ingresá un número entero válido.");
    }
    return valor;
}

static decimal PedirDecimal(string etiqueta)
{
    decimal valor = 0;
    bool valido = false;
    while (!valido)
    {
        Console.Write($"{etiqueta}: ");
        // Aceptamos punto o coma; parseamos con InvariantCulture (punto decimal).
        string entrada = (Console.ReadLine() ?? "").Replace(',', '.');
        valido = decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
        if (!valido)
            Console.WriteLine("Ingresá un número válido (ej. 39990.50).");
    }
    return valor;
}

static DateTime PedirFecha(string etiqueta)
{
    DateTime valor = default;
    bool valido = false;
    while (!valido)
    {
        Console.Write($"{etiqueta}: ");
        valido = DateTime.TryParse(Console.ReadLine(), out valor);
        if (!valido)
            Console.WriteLine("Ingresá una fecha válida (ej. 2026-12-31).");
    }
    return valor;
}

