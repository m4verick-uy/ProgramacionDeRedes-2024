namespace Common.Protocolo;

/// <summary>
/// Constantes del protocolo propietario, compartidas por Cliente y Servidor.
///
/// La letra sugiere una trama HEADER(3) + CMD(2) + LARGO(4) + DATOS.
/// Quitamos el campo HEADER (REQ/RES) por ser redundante: el CMD ya identifica
/// unívocamente la operación, y el sentido de la comunicación lo da el flujo
/// (el cliente pide, el servidor responde sobre el mismo socket). Menos bytes
/// en la red y menos parsing, con el mismo resultado.
///
/// Trama final:  CMD(2) + LARGO(4) + DATOS(variable)
/// </summary>
public static class ProtocoloConstantes
{
    // Largo fijo, en bytes, de cada campo de la cabecera de la trama.
    public const int LargoCmd = 2;      // Comando: valor 0-99 en texto ("01", "27", ...)
    public const int LargoLargo = 4;    // Largo del campo DATOS: entero de 4 bytes

    // Separador de campos DENTRO de DATOS no se usa: serializamos con el patrón
    // largo(4) + valor por cada campo (ver Paquete). Es robusto ante cualquier
    // contenido (tildes, emojis, saltos de línea, '#') sin necesidad de escapar.

    // Encoding único para todo texto que viaje por la red.
    // (System.Text.Encoding.UTF8 se usa directamente en las clases que serializan.)
}
