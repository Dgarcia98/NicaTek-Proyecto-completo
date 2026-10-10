using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _repository;

        public PermisoService(IPermisoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PermisoDTO>> ListarPermisos()
        {
            var data = await _repository.ListarPermisosAsync();
            return data.Select(p => new PermisoDTO
            {
                Id_Permiso = p.Id_Permiso,
                Nombre_Permiso = p.Nombre_Permiso,
                Codigo_Permiso = p.Codigo_Permiso,
                Descripcion = p.Descripcion,
                Id_Estado = p.Id_Estado
            });
        }

        public async Task<PermisoDTO> ObtenerPermisoPorId(int id)
        {
            var p = await _repository.ObtenerPermisoPorIdAsync(id);
            if (p == null) return null;

            return new PermisoDTO
            {
                Id_Permiso = p.Id_Permiso,
                Nombre_Permiso = p.Nombre_Permiso,
                Codigo_Permiso = p.Codigo_Permiso,
                Descripcion = p.Descripcion,
                Id_Estado = p.Id_Estado
            };
        }

        public async Task<IEnumerable<PermisoDTO>> FiltrarPermisos(string filtro)
        {
            var data = await _repository.FiltrarPermisosAsync(filtro);
            return data.Select(p => new PermisoDTO
            {
                Id_Permiso = p.Id_Permiso,
                Nombre_Permiso = p.Nombre_Permiso,
                Codigo_Permiso = p.Codigo_Permiso,
                Descripcion = p.Descripcion,
                Id_Estado = p.Id_Estado
            });
        }

        public async Task NuevoPermiso(PermisoDTO dto)
        {
            var entidad = new Permiso
            {
                Nombre_Permiso = dto.Nombre_Permiso,
                Codigo_Permiso = dto.Codigo_Permiso,
                Descripcion = dto.Descripcion,
                Id_Estado = dto.Id_Estado
            };
            await _repository.InsertarPermisoAsync(entidad);
        }

        public async Task EditarPermiso(PermisoDTO dto)
        {
            var entidad = new Permiso
            {
                Id_Permiso = dto.Id_Permiso,
                Nombre_Permiso = dto.Nombre_Permiso,
                Codigo_Permiso = dto.Codigo_Permiso,
                Descripcion = dto.Descripcion,
                Id_Estado = dto.Id_Estado
            };
            await _repository.ActualizarPermisoAsync(entidad);
        }

        public async Task EliminarPermiso(int id)
        {
            await _repository.EliminarPermisoAsync(id);
        }
    }
}