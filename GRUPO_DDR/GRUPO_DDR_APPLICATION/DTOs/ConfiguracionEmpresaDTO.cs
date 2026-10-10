using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class ConfiguracionEmpresaDTO
    {
        public int Id_Configuracion { get; set; }
        public string Nombre_Empresa { get; set; }
        public string RUC_No { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo_Empresa { get; set; }
        public string? Ruta_Logo { get; set; }
        public string Moneda_Simbolo { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int? Id_Modificador { get; set; }
    }
}