using System;
using System.ComponentModel.DataAnnotations;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Proveedores : EntidadBase
    {
        public int Id_Proveedor { get; set; }
        public int Id_Persona { get; set; }
        public DateTime FechaRegistro { get; set; } // Fecha contrato
        public DateTime? Fecha_Terminacion_Contrato { get; set; }
        public int Id_Estado { get; set; }
        public string Cedula { get; set; }
        public string Primer_Nombre { get; set; }
        public string? Segundo_Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string? Segundo_Apellido { get; set; }
        public string Estado { get; set; }

        public string Nombre_Completo
        {
            get
            {
                string sNombre = string.IsNullOrWhiteSpace(Segundo_Nombre) ? "" : " " + Segundo_Nombre;
                string sApellido = string.IsNullOrWhiteSpace(Segundo_Apellido) ? "" : " " + Segundo_Apellido;
                return $"{Primer_Nombre}{sNombre} {Primer_Apellido}{sApellido}".Trim();
            }
        }
    }
}