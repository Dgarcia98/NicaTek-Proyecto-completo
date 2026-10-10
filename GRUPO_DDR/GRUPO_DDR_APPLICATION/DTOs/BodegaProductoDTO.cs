using System;
namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class BodegaProductoDTO
    {
        public int Id_Bodega_Producto { get; set; }
        public int Id_Bodega { get; set; }
        public string Nombre_Bodega { get; set; }
        public int Id_Producto { get; set; }
        public string Codigo_Producto { get; set; }
        public string Nombre_Producto { get; set; }
        public decimal Precio_Producto { get; set; }
        public int Id_Marca { get; set; }
        public string Nombre_Marca { get; set; }
        public int Id_Modelo { get; set; }
        public string Nombre_Modelo { get; set; }
        public string Nombre_Categoria { get; set; }
        public int Cantidad { get; set; }
        public string Estado_Bodega { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}