using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class RolDTO
    {
        public int Id_Rol { get; set; }
        public string Nombre_Rol { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public string? EstadoActivo { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int? Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}