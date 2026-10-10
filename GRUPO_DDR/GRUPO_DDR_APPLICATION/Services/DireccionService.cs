using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class DireccionService : IDireccionService
    {
        private readonly IDireccionRepository _repository;

        public DireccionService(IDireccionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DireccionDTO>> ListarDirecciones()
        {
            var listar = await _repository.ListarDireccionesAsync();

            return listar.Select(item => new DireccionDTO
            {
                Id_Direccion = item.Id_Direccion,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Direccion_Completa = item.Direccion_Completa,
                Id_Tipo_Direccion = item.Id_Tipo_Direccion,
                Tipo_Direccion = item.Tipo_Direccion,
                Activo = item.Activo,
                EstadoActivo = item.Activo ? "Sí" : "No",
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<IEnumerable<DireccionDTO>> FiltrarDirecciones(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return Enumerable.Empty<DireccionDTO>();

            var lista = await _repository.FiltrarDireccionesAsync(filtro);

            return lista.Select(item => new DireccionDTO
            {
                Id_Direccion = item.Id_Direccion,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Direccion_Completa = item.Direccion_Completa,
                Id_Tipo_Direccion = item.Id_Tipo_Direccion,
                Tipo_Direccion = item.Tipo_Direccion,
                Activo = item.Activo,
                EstadoActivo = item.Activo ? "Sí" : "No",
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<DireccionDTO> ObtenerPorId(int id)
        {
            var lista = await _repository.FiltrarDireccionesAsync(id.ToString());
            var item = lista.FirstOrDefault();

            if (item == null) return null;

            return new DireccionDTO
            {
                Id_Direccion = item.Id_Direccion,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Direccion_Completa = item.Direccion_Completa,
                Id_Tipo_Direccion = item.Id_Tipo_Direccion,
                Tipo_Direccion = item.Tipo_Direccion,
                Activo = item.Activo,
                EstadoActivo = item.Activo ? "Sí" : "No",
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task InsertarDireccion(DireccionDTO dto)
        {
            var oDireccion = new Direccion
            {
                Id_Persona = dto.Id_Persona,
                Direccion_Completa = dto.Direccion_Completa,
                Id_Tipo_Direccion = dto.Id_Tipo_Direccion,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Creador,
                Activo = true
            };

            // CORREGIDO: Solo await, sin return
            await _repository.NuevaDireccionAsync(oDireccion);
        }

        public async Task ActualizarDireccion(DireccionDTO dto)
        {
            var oDireccion = new Direccion
            {
                Id_Direccion = dto.Id_Direccion,
                Id_Persona = dto.Id_Persona,
                Direccion_Completa = dto.Direccion_Completa,
                Id_Tipo_Direccion = dto.Id_Tipo_Direccion,
                Id_Modificador = dto.Id_Modificador,
                Activo = dto.Activo
            };

            // CORREGIDO: Solo await, sin return
            await _repository.EditarDireccionAsync(oDireccion);
        }

        public async Task EliminarDireccion(int id, int idModificador)
        {
            // CORREGIDO: Solo await, sin return
            await _repository.EliminarDireccionAsync(id, idModificador);
        }
    }
}