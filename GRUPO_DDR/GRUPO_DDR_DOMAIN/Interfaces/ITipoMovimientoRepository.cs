using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ITipoMovimientoRepository
    {

        Task<IEnumerable<TipoMovimiento>> ListarTipoMovimiento();
        Task<IEnumerable<TipoMovimiento>> ListarPorNombre(string filtro);
        Task NuevoTipoMovimiento(TipoMovimiento entidad);
        Task EditarTipoMovimiento(TipoMovimiento entidad);
        Task EliminarTipoMovimiento(int id, int idModificador);
    }
}