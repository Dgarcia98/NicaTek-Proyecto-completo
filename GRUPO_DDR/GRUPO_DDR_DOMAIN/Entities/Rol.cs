namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Rol : EntidadBase
    {
        public int Id_Rol { get; set; }
        public string Nombre_Rol { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
        public string EstadoActivo { get; set; }
        public int? Id_Creador { get; set; }
    }
}