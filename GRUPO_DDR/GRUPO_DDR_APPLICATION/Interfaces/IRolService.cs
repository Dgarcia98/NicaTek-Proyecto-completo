using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<RolDTO>> ListarRoles();
        Task<RolDTO> ObtenerRolPorId(int id);
        Task<IEnumerable<RolDTO>> ListarRolesFiltro(string filtro);
        Task NuevoRol(RolDTO dto);
        Task EditarRol(RolDTO dto);
        Task EliminarRol(int id, int idModificador);
    }
}