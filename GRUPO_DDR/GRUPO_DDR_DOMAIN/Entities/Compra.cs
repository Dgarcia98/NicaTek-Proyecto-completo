using System;
using System.ComponentModel.DataAnnotations;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Compra : EntidadBase
    {
        public int Id_Compra { get; set; }
        public int Id_Proveedor { get; set; }
        public string Proveedor { get; set; }
        public string Numero_Factura_Proveedor { get; set; }
        public DateTime Fecha_Compra { get; set; }
        public decimal Total_Compra { get; set; }
        public int Id_Estado { get; set; }
        public string Estado { get; set; } 
    }
}