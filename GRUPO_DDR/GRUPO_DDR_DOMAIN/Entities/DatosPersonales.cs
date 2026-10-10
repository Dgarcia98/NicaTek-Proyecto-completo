using System;

namespace GRUPO_DDR_DOMAIN.Entities
{
    public class DatosPersonales : EntidadBase
    {
        public int Id_Persona { get; set; }
        public string Primer_Nombre { get; set; }
        public string Segundo_Nombre { get; set; }
        public string Primer_Apellido { get; set; }
        public string Segundo_Apellido { get; set; }
        public int Id_Tipo_DNI { get; set; }
        public string DNI { get; set; }
        public int Id_Genero { get; set; }
        public int Id_Nacionalidad { get; set; }

    }
}