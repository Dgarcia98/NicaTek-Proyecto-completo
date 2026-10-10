using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IBodegaRepository
    {
        Task<IEnumerable<Bodegas>> ListarBodegaAsync();
        Task<IEnumerable<Bodegas>> ListarBodegaFiltroAsync(string filtro);
        Task NuevaBodegaAsync(Bodegas oBodega);
        Task EditarBodegaAsync(Bodegas oBodega);
        Task EliminarBodegaAsync(int id, int idModificador);
    }
}