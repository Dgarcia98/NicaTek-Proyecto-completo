using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IPermisoRepository
    {
        Task<IEnumerable<Permiso>> ListarPermisosAsync();
        Task<Permiso> ObtenerPermisoPorIdAsync(int id);
        Task<IEnumerable<Permiso>> FiltrarPermisosAsync(string filtro);
        Task InsertarPermisoAsync(Permiso permiso);
        Task ActualizarPermisoAsync(Permiso permiso);
        Task EliminarPermisoAsync(int id);
    }
}