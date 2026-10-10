using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class TipoCatalogoDTO
    {
        public int Id_Tipo_Catalogo { get; set; }
        public string Tipo_Catalogo { get; set; }
        public int Id_Creador { get; set; }
        public int Id_Modificador { get; set; }
        public bool Activo { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
    }
}