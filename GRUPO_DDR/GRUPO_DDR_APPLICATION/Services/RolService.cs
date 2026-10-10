using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _repository;

        public RolService(IRolRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RolDTO>> ListarRoles()
        {
            var roles = await _repository.ListarRolesAsync();
            return roles.Select(r => new RolDTO
            {
                Id_Rol = r.Id_Rol,
                Nombre_Rol = r.Nombre_Rol,
                Descripcion = r.Descripcion,
                Activo = r.Activo,
                EstadoActivo = r.EstadoActivo,
                Fecha_Creacion = r.Fecha_Creacion,
                Fecha_Modificacion = r.Fecha_Modificacion,
                Id_Creador = r.Id_Creador,
                Id_Modificador = r.Id_Modificador
            });
        }

        public async Task<RolDTO> ObtenerRolPorId(int id)
        {
            var roles = await _repository.ListarRolesFiltroAsync(id.ToString());
            var r = roles.FirstOrDefault(x => x.Id_Rol == id);

            if (r == null) return null;

            return new RolDTO
            {
                Id_Rol = r.Id_Rol,
                Nombre_Rol = r.Nombre_Rol,
                Descripcion = r.Descripcion,
                Activo = r.Activo,
                EstadoActivo = r.EstadoActivo,
                Fecha_Creacion = r.Fecha_Creacion,
                Fecha_Modificacion = r.Fecha_Modificacion,
                Id_Creador = r.Id_Creador,
                Id_Modificador = r.Id_Modificador
            };
        }

        public async Task<IEnumerable<RolDTO>> ListarRolesFiltro(string filtro)
        {
            var roles = await _repository.ListarRolesFiltroAsync(filtro);
            return roles.Select(r => new RolDTO
            {
                Id_Rol = r.Id_Rol,
                Nombre_Rol = r.Nombre_Rol,
                Descripcion = r.Descripcion,
                Activo = r.Activo,
                EstadoActivo = r.EstadoActivo,
                Fecha_Creacion = r.Fecha_Creacion,
                Fecha_Modificacion = r.Fecha_Modificacion,
                Id_Creador = r.Id_Creador,
                Id_Modificador = r.Id_Modificador
            });
        }

        public async Task NuevoRol(RolDTO dto)
        {
            var entidad = new Rol
            {
                Nombre_Rol = dto.Nombre_Rol,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.NuevoRolAsync(entidad);
        }

        public async Task EditarRol(RolDTO dto)
        {
            var entidad = new Rol
            {
                Id_Rol = dto.Id_Rol,
                Nombre_Rol = dto.Nombre_Rol,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.EditarRolAsync(entidad);
        }

        public async Task EliminarRol(int id, int idModificador)
        {
            await _repository.EliminarRolAsync(id, idModificador);
        }
    }
}