using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Cliente : EntidadBase
    {
        public int Id_Cliente { get; set; }
        public int Id_Persona { get; set; }
        public string Nombre_Completo { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
    }
}