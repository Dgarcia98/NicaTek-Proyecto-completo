namespace GRUPO_DDR_DOMAIN.Entities
{
    public class ConfiguracionEmpresa : EntidadBase
    { 
        public int Id_Configuracion { get; set; }
        public string Nombre_Empresa { get; set; }
        public string RUC_No { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo_Empresa { get; set; }
        public string Ruta_Logo { get; set; }
        public string Moneda_Simbolo { get; set; }

    }
}