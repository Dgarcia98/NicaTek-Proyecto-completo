using System;
using System.ComponentModel.DataAnnotations;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class DetalleVenta : EntidadBase
    {
        public int Id_Detalle { get; set; }
        public int Id_Venta { get; set; }
        public decimal Total_Venta_General { get; set; }
        public int Id_Producto { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio_Unitario_Al_Momento_Venta { get; set; }
        public decimal Subtotal { get; set; }
        public int Id_Estado { get; set; }
        public string Estado { get; set; }
    }
}