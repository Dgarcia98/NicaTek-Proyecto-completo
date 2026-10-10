using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class ClienteDTO
    {
        public int Id_Cliente { get; set; }
        public int Id_Persona { get; set; }
        public string Nombre_Completo { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public int Id_Creador { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int? Id_Modificador { get; set; }
        public bool Estado { get; set; }
    }
}