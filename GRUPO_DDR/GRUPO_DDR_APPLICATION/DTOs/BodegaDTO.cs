using System;
namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class BodegaDTO
    {
        public int Id_Bodega { get; set; }
        public string Nombre_Bodega { get; set; }
        public int Cantidad { get; set; }
        public int Total_Productos { get; set; }
        public int Id_Estado { get; set; }
        public string? Nombre_Estado { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }



}      