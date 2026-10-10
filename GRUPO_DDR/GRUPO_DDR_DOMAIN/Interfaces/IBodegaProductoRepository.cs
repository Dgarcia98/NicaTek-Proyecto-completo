using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IBodegaProductoRepository
    {
        Task<IEnumerable<BodegaProducto>> ListarAsync();
        Task<IEnumerable<BodegaProducto>> FiltrarAsync(string filtro);
        Task<IEnumerable<BodegaProducto>> ListarPorBodegaAsync(int idBodega);
        Task NuevoAsync(BodegaProducto entidad);
        Task EditarAsync(BodegaProducto entidad);
        Task EliminarAsync(int id, int idModificador);
    }
}