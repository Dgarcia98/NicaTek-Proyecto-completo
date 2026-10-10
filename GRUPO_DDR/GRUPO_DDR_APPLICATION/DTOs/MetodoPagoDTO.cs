using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class MetodoPagoDTO
    {
        public int Id_Metodo_Pago { get; set; }
        public string Descripcion { get; set; } 
        public int Id_Estado { get; set; }    
        public bool Activo { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}