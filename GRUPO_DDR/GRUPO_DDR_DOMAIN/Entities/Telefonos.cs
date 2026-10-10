namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Telefonos : EntidadBase
    {
        public int Id_Telefono { get; set; }
        public int Id_Persona { get; set; }
        public string Numero_Telefono { get; set; }
        public int? Id_Tipo_Telefono { get; set; }
        public string Tipo_Telefono { get; set; }
        public string Nombre_Estado { get; set; } 
    }
}