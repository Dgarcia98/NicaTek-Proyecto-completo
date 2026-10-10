using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface ICorreoService
    {
        Task<IEnumerable<CorreoDTO>> ListarCorreo();
        Task<CorreoDTO> ObtenerCorreoPorId(int id);
        Task<IEnumerable<CorreoDTO>> ListarPorFiltro(string buscar);
        Task NuevoCorreo(CorreoDTO dto);
        Task EditarCorreo(CorreoDTO dto);
        Task EliminarCorreo(int id, int idModificador);
    }
}