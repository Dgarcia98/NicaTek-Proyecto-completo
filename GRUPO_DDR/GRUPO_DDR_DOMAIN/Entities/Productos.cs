using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Productos : EntidadBase
    {
        public int Id_Producto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal Precio_Compra { get; set; }

        public int Id_Marca_Modelo { get; set; }
        public int? Id_Proveedor { get; set; }
        public int? Id_Categoria { get; set; }
        public int Id_Estado { get; set; }
        public int Id_Bodega { get; set; }
        public string Nombre_Marca { get; set; }
        public string Nombre_Modelo { get; set; }
        public string Nombre_Proveedor { get; set; }
    }
}