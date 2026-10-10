using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class Marca : EntidadBase
    {
        public int Id_Marca { get; set; }
        public string Nombre { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
    }
}