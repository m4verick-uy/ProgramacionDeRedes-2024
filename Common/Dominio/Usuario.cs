namespace Common.Dominio;

/// <summary>
/// Usuario del sistema (CR1). El nombre de usuario es único.
/// La contraseña se guarda en texto plano para simplificar: el foco del
/// obligatorio es sockets/threads/protocolo, no seguridad.
/// </summary>
public class Usuario
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
}
