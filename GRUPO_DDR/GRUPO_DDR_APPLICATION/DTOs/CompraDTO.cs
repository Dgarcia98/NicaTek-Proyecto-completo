using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class CompraDTO
    {
        public int Id_Compra { get; set; }
        public int Id_Proveedor { get; set; }
        public string Proveedor { get; set; }
        public string Numero_Factura_Proveedor { get; set; }
        public DateTime Fecha_Compra { get; set; }
        public decimal Total_Compra { get; set; }
        public int Id_Estado { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}