using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Usuario : EntidadBase
    {
        // Campos propios de la tabla Tbl_Usuarios
        public int Id_Usuario { get; set; }
        public string Nombre_Usuario { get; set; }
        public string Contrasena { get; set; }
        public int Id_Persona { get; set; }
        public int Id_Rol { get; set; }
        public DateTime? Fecha_Ultimo_Acceso { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Persona { get; set; }
        public string Nombre_Rol { get; set; }
        public string Estado_Nombre { get; set; }
    }
}