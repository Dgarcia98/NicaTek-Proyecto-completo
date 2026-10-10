
namespace GRUPO_DDR_DOMAIN.Entities
{
    public abstract class EntidadBase
    {
        public DateTime Fecha_Creacion { get; set; }
        public int Id_Creador { get; set; }
        public DateTime? Fecha_Modificacion { get; set; }
        public int? Id_Modificador { get; set; }
        public bool Estado { get; set; } = true;

    }
}