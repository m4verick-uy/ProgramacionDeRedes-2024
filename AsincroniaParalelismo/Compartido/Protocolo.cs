using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class Protocolo
{
    // ---------- Versión sincrónica ----------
    public static void Enviar(NetworkStream stream, string mensaje)
    {
        byte[] cuerpo = Encoding.UTF8.GetBytes(mensaje);
        byte[] largo = BitConverter.GetBytes(cuerpo.Length);
        stream.Write(largo, 0, largo.Length);
        stream.Write(cuerpo, 0, cuerpo.Length);
    }

    public static string Recibir(NetworkStream stream)
    {
        byte[] largo = LeerExacto(stream, 4);
        byte[] cuerpo = LeerExacto(stream, BitConverter.ToInt32(largo, 0));
        return Encoding.UTF8.GetString(cuerpo);
    }

    private static byte[] LeerExacto(NetworkStream stream, int cantidad)
    {
        byte[] buffer = new byte[cantidad];
        int leidos = 0;
        while (leidos < cantidad)
        {
            int n = stream.Read(buffer, leidos, cantidad - leidos);
            if (n == 0) throw new IOException("La conexión se cerró");
            leidos += n;
        }
        return buffer;
    }

    // ---------- Versión asincrónica ----------
    public static async Task EnviarAsync(NetworkStream stream, string mensaje,
        CancellationToken token = default)
    {
        byte[] cuerpo = Encoding.UTF8.GetBytes(mensaje);
        byte[] largo = BitConverter.GetBytes(cuerpo.Length);
        await stream.WriteAsync(largo, 0, largo.Length, token);
        await stream.WriteAsync(cuerpo, 0, cuerpo.Length, token);
    }

    public static async Task<string> RecibirAsync(NetworkStream stream,
        CancellationToken token = default)
    {
        byte[] largo = await LeerExactoAsync(stream, 4, token);
        int tamanio = BitConverter.ToInt32(largo, 0);
        byte[] cuerpo = await LeerExactoAsync(stream, tamanio, token);
        return Encoding.UTF8.GetString(cuerpo);
    }

    private static async Task<byte[]> LeerExactoAsync(NetworkStream stream,
        int cantidad, CancellationToken token)
    {
        byte[] buffer = new byte[cantidad];
        int leidos = 0;
        while (leidos < cantidad)
        {
            int n = await stream.ReadAsync(buffer, leidos, cantidad - leidos,
                token);
            if (n == 0) throw new IOException("La conexión se cerró");
            leidos += n;
        }
        return buffer;
    }
}
