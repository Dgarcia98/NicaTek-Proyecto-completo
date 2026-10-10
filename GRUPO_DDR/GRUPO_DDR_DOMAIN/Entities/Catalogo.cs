namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Catalogo : EntidadBase
    {
        public int Id_Catalogo { get; set; }
        public int Id_Tipo_Catalogo { get; set; }
        public string Tipo_Catalogo_Nombre { get; set; }
        public string Nombre_Catalogo { get; set; }
        public bool Activo { get; set; }
    }
}