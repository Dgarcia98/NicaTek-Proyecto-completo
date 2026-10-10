using System;
using System.ComponentModel.DataAnnotations;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Venta : EntidadBase
    {
        public int Id_Venta { get; set; }
        public DateTime Fecha { get; set; }
        public int Id_Cliente { get; set; }
        public string Cliente { get; set; }
        public int? Id_Empleado { get; set; }
        public string Empleado { get; set; }
        public int? Id_Metodo_Pago { get; set; }
        public string Metodo_Pago { get; set; }
        public string Numero_Factura { get; set; }
        public decimal Total { get; set; }
        public int Id_Estado { get; set; }
        public string Estado { get; set; }
    }
}