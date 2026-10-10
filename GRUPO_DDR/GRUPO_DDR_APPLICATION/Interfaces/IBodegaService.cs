using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IBodegaService
    {
        Task<IEnumerable<BodegaDTO>> ListarBodega();
        Task<BodegaDTO> ObtenerBodegaPorId(int id);
        Task<IEnumerable<BodegaDTO>> ListarPorNombre(string buscar);
        Task NuevoBodega(BodegaDTO dto);
        Task EditarBodega(BodegaDTO dto);
        Task EliminarBodega(int id, int idModificador);
    }
}