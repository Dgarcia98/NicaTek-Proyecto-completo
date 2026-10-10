using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class VentaDTO
    {
        public int Id_Venta { get; set; }
        public DateTime Fecha { get; set; }

        public int Id_Cliente { get; set; }
        public string? Cliente { get; set; } // <-- Añadido el ?

        public int? Id_Empleado { get; set; }
        public string? Empleado { get; set; } // <-- Añadido el ?

        public int? Id_Metodo_Pago { get; set; }
        public string? Metodo_Pago { get; set; } // <-- ¡AQUÍ ESTÁ EL CULPABLE! Añadido el ?

        public decimal Total { get; set; }

        public int Id_Estado { get; set; }
        public string? Estado { get; set; } // <-- Añadido el ?

        public DateTime Fecha_Creacion { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int Id_Creador { get; set; }
        public int? Id_Modificador { get; set; }
    }
}