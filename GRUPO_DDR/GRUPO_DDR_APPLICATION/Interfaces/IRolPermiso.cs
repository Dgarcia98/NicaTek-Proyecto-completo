using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IRolPermisoService
    {
        Task<IEnumerable<RolPermisoDTO>> ListarPermisosPorRol(int idRol);
        Task AsignarPermiso(int idRol, int idPermiso);
        Task RemoverPermiso(int idRol, int idPermiso);
    }
}