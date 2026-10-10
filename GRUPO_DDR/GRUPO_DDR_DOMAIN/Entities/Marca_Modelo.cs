namespace GRUPO_DDR_DOMAIN.Entities
{
    public class MarcaModelo : EntidadBase
    {
        public int Id_Marca_Modelo { get; set; }
        public int Id_Marca { get; set; }
        public string Nombre_Marca { get; set; } // Propiedad extendida (viene del JOIN)
        public int Id_Modelo { get; set; }
        public string Nombre_Modelo { get; set; } // Propiedad extendida (viene del JOIN)
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
    }
}