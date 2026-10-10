using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IEstadoService
    {
        Task<IEnumerable<EstadoDTO>> ListarEstado();
        Task<EstadoDTO> ObtenerEstadoPorId(int id);
        Task<IEnumerable<EstadoDTO>> ListarPorNombre(string Buscar);
        Task NuevoEstado(EstadoDTO dto);
        Task EditarEstado(EstadoDTO dto);
        Task EliminarEstado(int id, int idModificador);
    }
}