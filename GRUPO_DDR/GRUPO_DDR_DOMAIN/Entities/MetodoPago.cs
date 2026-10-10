namespace GRUPO_DDR_DOMAIN.Entities
{
    public class MetodoPago : EntidadBase
    {
        public int Id_Metodo_Pago { get; set; }
        public string Descripcion { get; set; }
        public int Id_Estado { get; set; }     
        public bool Activo { get; set; }
    }
}