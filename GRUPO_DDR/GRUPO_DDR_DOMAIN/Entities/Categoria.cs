namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Categoria : EntidadBase
    {
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
    }
}