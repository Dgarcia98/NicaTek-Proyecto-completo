using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class DireccionDTO
    {
        public int Id_Direccion { get; set; }
        public int Id_Persona { get; set; }

        public string Nombre_Completo { get; set; }
        public string Tipo_Direccion { get; set; }
        public string EstadoActivo { get; set; }

        public string Direccion_Completa { get; set; }
        public int Id_Tipo_Direccion { get; set; }
        public bool Activo { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; } 
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; } 
    }
}