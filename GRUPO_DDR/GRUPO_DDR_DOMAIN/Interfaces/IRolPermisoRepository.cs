using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IRolPermisoRepository
    {
        Task<IEnumerable<RolPermiso>> ListarPermisosPorRolAsync(int idRol);
        Task AsignarPermisoAsync(int idRol, int idPermiso);
        Task RemoverPermisoAsync(int idRol, int idPermiso);
    }
}