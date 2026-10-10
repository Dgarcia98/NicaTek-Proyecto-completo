using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IPermisoService
    {
        Task<IEnumerable<PermisoDTO>> ListarPermisos();
        Task<PermisoDTO> ObtenerPermisoPorId(int id);
        Task<IEnumerable<PermisoDTO>> FiltrarPermisos(string filtro);
        Task NuevoPermiso(PermisoDTO dto);
        Task EditarPermiso(PermisoDTO dto);
        Task EliminarPermiso(int id);
    }
}