public class UsuarioDTO
{
    public int Id_Usuario { get; set; }
    public string Nombre_Usuario { get; set; } // Obligatorio
    public string Contrasena { get; set; }     // Obligatorio
    public int Id_Persona { get; set; }
    public int Id_Rol { get; set; }
    public int Id_Estado { get; set; }
    public int Id_Creador { get; set; }

    // --- CAMPOS QUE DEBEN SER OPCIONALES (NULLABLES) ---
    // Agrega el '?' después del tipo de dato string
    public string? Token { get; set; }

    /// <summary>
    /// Codigos de permiso del usuario. Se devuelven solo para que la UI
    /// sepa que botones mostrar. NO son la fuente de autorizacion: esa es
    /// el claim firmado dentro del token, que el servidor valida en cada
    /// request. Manipular esta lista en el navegador no concede nada.
    /// </summary>
    public List<string>? Permisos { get; set; }

    public string? Nombre_Rol { get; set; }
    public string? Estado_Nombre { get; set; }
    public string? Nombre_Persona { get; set; }

    public DateTime? Fecha_Ultimo_Acceso { get; set; }
    public DateTime? Fecha_Creacion { get; set; }
    public DateTime? Fecha_Modificacion { get; set; }
    public int? Id_Modificador { get; set; }
}