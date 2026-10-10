using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IMovimientoInventarioRepository
    {
        Task<IEnumerable<MovimientoInventario>> ListarMovimientosAsync();
        Task<IEnumerable<MovimientoInventario>> ListarMovimientosFiltroAsync(string filtro);
        Task NuevoMovimientoAsync(MovimientoInventario movimiento);
    }
}