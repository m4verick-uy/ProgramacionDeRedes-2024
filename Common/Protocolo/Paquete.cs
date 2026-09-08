using System.Text;

namespace Common.Protocolo;

/// <summary>
/// Serialización MANUAL del contenido del campo DATOS, campo por campo.
///
/// Prohibido usar BinaryFormatter y parsers como JSON (ver letra). Armamos el
/// protocolo a mano: cada campo se escribe como  largo(4 bytes) + valor(bytes).
/// Para leer, el receptor lee 4 bytes (el largo), y luego esa cantidad de bytes
/// (el valor). El orden de los campos lo define cada comando y es un contrato
/// implícito entre Cliente y Servidor.
///
/// Uso escritura:
///     var p = new Paquete();
///     p.EscribirString("Model 3");
///     p.EscribirInt(39990);
///     byte[] datos = p.ObtenerBytes();
///
/// Uso lectura:
///     var p = new Paquete(datos);
///     string nombre = p.LeerString();
///     int precio   = p.LeerInt();
/// </summary>
public class Paquete
{
    private readonly List<byte> _buffer;   // se usa al ESCRIBIR
    private readonly byte[] _datos;        // se usa al LEER
    private int _posicion;                 // cursor de lectura

    // Constructor para ESCRIBIR un paquete nuevo.
    public Paquete()
    {
        _buffer = new List<byte>();
        _datos = Array.Empty<byte>();
        _posicion = 0;
    }

    // Constructor para LEER un paquete recibido.
    public Paquete(byte[] datos)
    {
        _buffer = new List<byte>();
        _datos = datos;
        _posicion = 0;
    }

    // ---------------- Escritura ----------------

    public void EscribirString(string valor)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(valor ?? string.Empty);
        EscribirBytes(bytes);
    }

    public void EscribirInt(int valor)
    {
        // Un int viaja como su texto decimal, así queda legible y es un campo más.
        EscribirString(valor.ToString());
    }

    public void EscribirDecimal(decimal valor)
    {
        // Cultura invariante para que "1234.50" se lea igual en cualquier máquina.
        EscribirString(valor.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }

    public void EscribirDateTime(DateTime valor)
    {
        // ISO 8601 redondeado ("o") -> se parsea sin ambigüedad de formato regional.
        EscribirString(valor.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
    }

    // Escribe un campo binario crudo (largo + bytes). Base de todos los demás.
    public void EscribirBytes(byte[] valor)
    {
        valor ??= Array.Empty<byte>();
        _buffer.AddRange(BitConverter.GetBytes(valor.Length)); // largo(4)
        _buffer.AddRange(valor);                               // valor
    }

    public byte[] ObtenerBytes() => _buffer.ToArray();

    // ---------------- Lectura ----------------

    public string LeerString() => Encoding.UTF8.GetString(LeerBytes());

    public int LeerInt() => int.Parse(LeerString());

    public decimal LeerDecimal() =>
        decimal.Parse(LeerString(), System.Globalization.CultureInfo.InvariantCulture);

    public DateTime LeerDateTime() =>
        DateTime.Parse(LeerString(), System.Globalization.CultureInfo.InvariantCulture,
                       System.Globalization.DateTimeStyles.RoundtripKind);

    // Lee un campo binario crudo: 4 bytes de largo, luego esa cantidad de bytes.
    public byte[] LeerBytes()
    {
        int largo = BitConverter.ToInt32(_datos, _posicion);
        _posicion += 4;

        byte[] valor = new byte[largo];
        Array.Copy(_datos, _posicion, valor, 0, largo);
        _posicion += largo;

        return valor;
    }

    // ¿Quedan campos por leer? (útil para listas de largo variable)
    public bool HayMasDatos() => _posicion < _datos.Length;
}
