using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IRolRepository
    {
        Task<IEnumerable<Rol>> ListarRolesAsync();
        Task<IEnumerable<Rol>> ListarRolesFiltroAsync(string filtro);
        Task NuevoRolAsync(Rol rol);
        Task EditarRolAsync(Rol rol);
        Task EliminarRolAsync(int id, int idModificador);
    }
}