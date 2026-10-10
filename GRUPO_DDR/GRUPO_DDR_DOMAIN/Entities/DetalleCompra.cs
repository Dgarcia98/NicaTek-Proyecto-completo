using System;
using System.ComponentModel.DataAnnotations;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class DetalleCompra : EntidadBase
    {

        public int Id_Detalle_Compra { get; set; }
        public int Id_Compra { get; set; }
        public int Id_Bodega { get; set; }

        public decimal Total_Compra { get; set; }
        public int Id_Producto { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio_Unitario_Compra { get; set; }
        public decimal Subtotal_Compra { get; set; }
    }
}