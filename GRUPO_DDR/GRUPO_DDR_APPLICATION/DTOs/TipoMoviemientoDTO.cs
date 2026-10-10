using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class TipoMovimientoDTO
    {
        public int Id_Tipo_Movimiento { get; set; }
        public string Descripcion { get; set; }
        public string Operacion { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
        public bool Activo { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
    }
}