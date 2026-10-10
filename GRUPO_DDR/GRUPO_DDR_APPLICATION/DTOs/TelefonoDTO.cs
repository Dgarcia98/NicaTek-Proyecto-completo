using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class TelefonoDTO
    {
        public int Id_Telefono { get; set; }
        public int Id_Persona { get; set; }
        public string Numero_Telefono { get; set; }
        public int? Id_Tipo_Telefono { get; set; }
        public string? Tipo_Telefono { get; set; } 
        public bool Estado { get; set; }
        public string? Nombre_Estado { get; set; } 
        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}