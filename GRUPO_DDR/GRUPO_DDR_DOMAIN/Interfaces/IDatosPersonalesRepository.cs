using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IDatosPersonalesRepository
    {
        Task<IEnumerable<DatosPersonales>> ObtenerTodosAsync();
        Task<IEnumerable<DatosPersonales>> ObtenerPorFiltroAsync(string filtro);
        Task InsertarAsync(DatosPersonales entidad);
        Task ActualizarAsync(DatosPersonales entidad);
        Task EliminarAsync(int idPersona);
    }
}