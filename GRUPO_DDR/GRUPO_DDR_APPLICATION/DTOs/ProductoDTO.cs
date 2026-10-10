namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class ProductoDTO
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
        public string Nombre_Marca { get; set; }
        public string Nombre_Modelo { get; set; }
        public string Proveedor { get; set; }
        public string Categoria { get; set; }
        public int Id_Creador { get; set; }
        public int Id_Modificador { get; set; }
        public string Estado { get; set; }
    }
}