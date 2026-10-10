namespace GRUPO_DDR_DOMAIN.Entities
{
    public class RolPermiso : EntidadBase
    {
        public int Id_Rol_Permiso { get; set; }
        public int Id_Rol { get; set; }
        public string Nombre_Rol { get; set; }
        public int Id_Permiso { get; set; }
        public string Nombre_Permiso { get; set; }
        public string Codigo_Permiso { get; set; }
        public string Descripcion_Permiso { get; set; }
        public bool Activo { get; set; }
    }
}