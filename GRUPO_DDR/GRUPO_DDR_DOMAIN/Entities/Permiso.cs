namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Permiso : EntidadBase
    {
        public int Id_Permiso { get; set; }
        public string Nombre_Permiso { get; set; }
        public string Codigo_Permiso { get; set; }
        public string Descripcion { get; set; }
        public int Id_Estado { get; set; }
    }
}