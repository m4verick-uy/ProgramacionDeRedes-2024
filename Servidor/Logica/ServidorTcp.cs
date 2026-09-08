using System.Net;
using System.Net.Sockets;
using Servidor.Almacenamiento;

namespace Servidor.Logica;

/// <summary>
/// Servidor TCP. Escucha conexiones y lanza un Thread por cada cliente (SR2).
///
/// Cierre controlado (SR4): una bandera detiene la aceptación de nuevos clientes,
/// se cierra el socket de escucha y ADEMÁS se cierran los sockets de los clientes
/// activos, para "finalizar las conexiones activas" como pide la letra.
/// </summary>
public class ServidorTcp
{
    private readonly Almacen _almacen;
    private readonly LogicaNegocio _logica;
    private Socket? _socketEscucha;

    // Sockets de clientes actualmente conectados. Se accede desde varios threads,
    // asi que se protege con su propio candado.
    private readonly List<Socket> _clientesActivos = new();
    private readonly object _candadoClientes = new();

    // Bandera de ejecucion. volatile: la leen/escriben distintos threads
    // (el de escucha, los de clientes y el que dispara el cierre).
    private volatile bool _enEjecucion;

    public bool EnEjecucion => _enEjecucion;

    public ServidorTcp(Almacen almacen, LogicaNegocio logica)
    {
        _almacen = almacen;
        _logica = logica;
    }

    public void Iniciar()
    {
        _socketEscucha = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var endpoint = new IPEndPoint(IPAddress.Parse(Configuracion.Ip), Configuracion.Puerto);
        _socketEscucha.Bind(endpoint);
        _socketEscucha.Listen(Configuracion.Backlog);
        _enEjecucion = true;

        Console.WriteLine($"[Servidor] Escuchando en {Configuracion.Ip}:{Configuracion.Puerto}");
        Console.WriteLine("[Servidor] Esperando clientes...");

        while (_enEjecucion)
        {
            try
            {
                Socket socketCliente = _socketEscucha.Accept(); // bloqueante
                Console.WriteLine("[Servidor] Cliente conectado.");

                RegistrarCliente(socketCliente);

                // Un Thread por cliente para no bloquear al resto (SR2).
                var manejador = new ManejadorCliente(socketCliente, _logica, _almacen, this);
                new Thread(manejador.Atender).Start();
            }
            catch (SocketException)
            {
                // Al hacer el cierre controlado se cierra el socket de escucha y
                // Accept lanza SocketException. Si ya no estamos en ejecucion,
                // es lo esperado y salimos del bucle.
                if (!_enEjecucion)
                    break;
            }
        }

        Console.WriteLine("[Servidor] Bucle de aceptacion finalizado.");
    }

    // Cierre controlado (SR4): dejar de aceptar, cerrar el listener y cerrar los
    // sockets de los clientes activos.
    public void Detener()
    {
        _enEjecucion = false;

        try
        {
            _socketEscucha?.Close(); // desbloquea el Accept()
        }
        catch (SocketException) { /* ignorable */ }

        // Cerrar cada cliente activo. Su handler esta bloqueado en Receive; al
        // cerrar el socket, Receive corta y el handler termina limpiamente.
        lock (_candadoClientes)
        {
            foreach (var socket in _clientesActivos)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (SocketException) { /* ya podia estar cerrado */ }
                catch (ObjectDisposedException) { /* ya cerrado */ }
            }
            _clientesActivos.Clear();
        }

        Console.WriteLine("[Servidor] Cierre controlado completado.");
    }

    // Los manejadores registran/desregistran su socket para el cierre controlado.
    public void RegistrarCliente(Socket socket)
    {
        lock (_candadoClientes) { _clientesActivos.Add(socket); }
    }

    public void DesregistrarCliente(Socket socket)
    {
        lock (_candadoClientes) { _clientesActivos.Remove(socket); }
    }

    // CR5: procesar una reserva tras N segundos, en un Thread aparte para no
    // bloquear la atencion del cliente que la solicito.
    public void ProgramarProcesoReserva(int reservaId)
    {
        int segundos = Configuracion.SegundosProcesarReserva;
        new Thread(() =>
        {
            Thread.Sleep(segundos * 1000);
            _logica.ProcesarReserva(reservaId);
        }).Start();
    }
}
