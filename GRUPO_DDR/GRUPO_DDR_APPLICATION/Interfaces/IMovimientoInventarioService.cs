using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IMovimientoInventarioService
    {
        Task<IEnumerable<MovimientoInventarioDTO>> ListarMovimientos();
        Task<IEnumerable<MovimientoInventarioDTO>> ListarMovimientosFiltro(string filtro);
        Task NuevoMovimiento(MovimientoInventarioDTO dto);
    }
}