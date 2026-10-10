using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class TipoMovimiento : EntidadBase
    {
        public int Id_Tipo_Movimiento { get; set; }
        public string Descripcion { get; set; }
        public string Operacion { get; set; }
        public int Id_Estado { get; set; }
        public string Nombre_Estado { get; set; }
        public bool Activo { get; set; }
    }
}