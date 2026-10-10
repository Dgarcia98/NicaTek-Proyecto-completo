using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface ITipoMovimientoService
    {
        Task<IEnumerable<TipoMovimientoDTO>> ListarTipoMovimiento();

        Task<TipoMovimientoDTO> ObtenerTipoMovimientoPorId(int id);
        Task<IEnumerable<TipoMovimientoDTO>> ListarPorNombre(string Buscar);
        Task NuevoTipoMovimiento(TipoMovimientoDTO dto);
        Task EditarTipoMovimiento(TipoMovimientoDTO dto);
        Task EliminarTipoMovimiento(int id);
    }
}