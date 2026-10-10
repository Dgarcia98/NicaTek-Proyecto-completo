using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class EmpleadoDTO
    {
        public int Id_Empleado { get; set; }
        public int Id_Persona { get; set; }
        public string Nombre_Completo { get; set; }
        public string Cedula { get; set; }
        public DateTime Fecha_Contratacion { get; set; }
        public DateTime? Fecha_Despido { get; set; }
        public int Id_Cargo { get; set; }
        public string Cargo { get; set; }
        public int Id_Estado { get; set; }
        public string Estado { get; set; }

        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}