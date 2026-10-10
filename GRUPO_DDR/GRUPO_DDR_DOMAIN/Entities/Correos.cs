namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Correos : EntidadBase
    {
        public int Id_Correo { get; set; }
        public int Id_Persona { get; set; }
        public string Direccion_Correo { get; set; }
        public int? Id_Tipo_Correo { get; set; }
        public bool Activo { get; set; }
        public string Tipo_Correo { get; set; }
        public string Nombre_Completo { get; set; }
        public string EstadoActivo { get; set; }
    }
}