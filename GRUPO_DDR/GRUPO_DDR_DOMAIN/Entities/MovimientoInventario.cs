using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class MovimientoInventario : EntidadBase
    {
        public int Id_Movimiento { get; set; }
        public DateTime Fecha_Movimiento { get; set; }
        public int Id_Producto { get; set; }
        public string Producto { get; set; }
        public int Id_Tipo_Movimiento { get; set; }
        public string Tipo_Movimiento { get; set; }
        public int Cantidad { get; set; }
        public int Stock_Anterior { get; set; }
        public int Stock_Actual { get; set; }
        public int Id_Bodega { get; set; }
        public decimal Costo_Unitario { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
        public int Id_Usuario_Responsable { get; set; }
        public string Responsable { get; set; }
    }
}