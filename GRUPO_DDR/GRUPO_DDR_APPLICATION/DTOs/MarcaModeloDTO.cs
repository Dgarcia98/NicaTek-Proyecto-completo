using System;

namespace GRUPO_DDR_APPLICATION.DTOs
{
    public class MarcaModeloDTO
    {
        public int Id_Marca_Modelo { get; set; }
        public int Id_Marca { get; set; }
        public string Nombre_Marca { get; set; } // Para mostrar en el Frontend
        public int Id_Modelo { get; set; }
        public string Nombre_Modelo { get; set; } // Para mostrar en el Frontend
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
        public int Id_Creador { get; set; }
        public int Id_Modificador { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public DateTime Fecha_Modificacion { get; set; }
    }
}