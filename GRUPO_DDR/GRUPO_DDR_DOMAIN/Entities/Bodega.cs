namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Bodegas : EntidadBase
    {
        public int Id_Bodega { get; set; }
        public string Nombre_Bodega { get; set; }
        public int Cantidad { get; set; }          // Stock_Total del SP
        public int Total_Productos { get; set; }   // cantidad de productos distintos
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
    }
}