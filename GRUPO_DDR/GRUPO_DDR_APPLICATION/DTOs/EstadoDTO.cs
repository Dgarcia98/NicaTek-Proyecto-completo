using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class EstadoDTO
    {
        public int Id_Estado { get; set; }
        public string Nombre { get; set; } 
        public bool Estado { get; set; }   
        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}