using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class ProveedorDTO
    {
        public int Id_Proveedor { get; set; }
        public int Id_Persona { get; set; }

        public string Cedula { get; set; }
        public string Primer_Nombre { get; set; }
        public string? Segundo_Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string? Segundo_Apellido { get; set; }

        // Nullables porque al crear un proveedor no enviamos estos datos, se generan después
        public string? Nombre_Completo { get; set; }

        public int Id_Estado { get; set; }
        public string? Estado { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? Fecha_Terminacion_Contrato { get; set; }
    }
}