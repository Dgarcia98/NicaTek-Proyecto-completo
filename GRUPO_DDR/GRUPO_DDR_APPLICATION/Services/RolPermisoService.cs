using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class RolPermisoService : IRolPermisoService
    {
        private readonly IRolPermisoRepository _repository;

        public RolPermisoService(IRolPermisoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RolPermisoDTO>> ListarPermisosPorRol(int idRol)
        {
            var permisos = await _repository.ListarPermisosPorRolAsync(idRol);
            return permisos.Select(p => new RolPermisoDTO
            {
                Id_Rol_Permiso = p.Id_Rol_Permiso,
                Id_Rol = p.Id_Rol,
                Nombre_Rol = p.Nombre_Rol,
                Id_Permiso = p.Id_Permiso,
                Nombre_Permiso = p.Nombre_Permiso,
                Codigo_Permiso = p.Codigo_Permiso,
                Descripcion_Permiso = p.Descripcion_Permiso,
                Activo = p.Activo
            });
        }

        public async Task AsignarPermiso(int idRol, int idPermiso)
        {
            await _repository.AsignarPermisoAsync(idRol, idPermiso);
        }

        public async Task RemoverPermiso(int idRol, int idPermiso)
        {
            await _repository.RemoverPermisoAsync(idRol, idPermiso);
        }
    }
}