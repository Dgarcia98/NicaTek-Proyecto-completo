using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Direccion : EntidadBase
    {
        public int Id_Direccion { get; set; }
        public int Id_Persona { get; set; }
        public string Direccion_Completa { get; set; }
        public int Id_Tipo_Direccion { get; set; }
        public bool Activo { get; set; }
        public string Tipo_Direccion { get; set; } 
        public string Nombre_Completo { get; set; } 
        public string EstadoActivo { get; set; } 
    }
}