using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IDireccionService
    {
        Task<IEnumerable<DireccionDTO>> ListarDirecciones();
        Task<IEnumerable<DireccionDTO>> FiltrarDirecciones(string filtro);
        Task<DireccionDTO> ObtenerPorId(int id);
        Task InsertarDireccion(DireccionDTO dto);
        Task ActualizarDireccion(DireccionDTO dto);
        Task EliminarDireccion(int id, int idModificador);
    }
}