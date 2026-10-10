using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Empleado : EntidadBase
    {

        public int Id_Empleado { get; set; }
        public int Id_Persona { get; set; }
        public DateTime Fecha_Contratacion { get; set; }
        public DateTime? Fecha_Despido { get; set; } 
        public int Id_Cargo { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Completo { get; set; }
        public string Cedula { get; set; }
        public string Cargo { get; set; }
        public string Estado { get; set; }
    }
}