using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class CorreoService : ICorreoService
    {
        private readonly ICorreoRepository _repository;

        public CorreoService(ICorreoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CorreoDTO>> ListarCorreo()
        {
            var listar = await _repository.ListarCorreoAsync();

            return listar.Select(x => new CorreoDTO
            {
                Id_Correo = x.Id_Correo,
                Id_Persona = x.Id_Persona,
                Nombre_Completo = x.Nombre_Completo,
                Direccion_Correo = x.Direccion_Correo,
                Id_Tipo_Correo = x.Id_Tipo_Correo,
                Tipo_Correo = x.Tipo_Correo,
                Activo = x.Activo,
                EstadoActivo = x.EstadoActivo,
                Fecha_Creacion = x.Fecha_Creacion,
                Fecha_Modificacion = x.Fecha_Modificacion,
                Id_Creador = x.Id_Creador,
                Id_Modificador = x.Id_Modificador
            });
        }

        public async Task<IEnumerable<CorreoDTO>> ListarPorFiltro(string buscar)
        {
            if (string.IsNullOrWhiteSpace(buscar))
                return Enumerable.Empty<CorreoDTO>();

            var lista = await _repository.ListarCorreoFiltroAsync(buscar);

            return lista.Select(x => new CorreoDTO
            {
                Id_Correo = x.Id_Correo,
                Id_Persona = x.Id_Persona,
                Nombre_Completo = x.Nombre_Completo,
                Direccion_Correo = x.Direccion_Correo,
                Id_Tipo_Correo = x.Id_Tipo_Correo,
                Tipo_Correo = x.Tipo_Correo,
                Activo = x.Activo,
                EstadoActivo = x.EstadoActivo,
                Fecha_Creacion = x.Fecha_Creacion,
                Fecha_Modificacion = x.Fecha_Modificacion,
                Id_Creador = x.Id_Creador,
                Id_Modificador = x.Id_Modificador
            });
        }

        public async Task<CorreoDTO> ObtenerCorreoPorId(int id)
        {
            // Usamos el filtro para buscar por ID, ya que el SP suele manejarlo así o el repo lo adapta
            var lista = await _repository.ListarCorreoFiltroAsync(id.ToString());
            var x = lista.FirstOrDefault();

            if (x == null) return null;

            return new CorreoDTO
            {
                Id_Correo = x.Id_Correo,
                Id_Persona = x.Id_Persona,
                Nombre_Completo = x.Nombre_Completo,
                Direccion_Correo = x.Direccion_Correo,
                Id_Tipo_Correo = x.Id_Tipo_Correo,
                Tipo_Correo = x.Tipo_Correo,
                Activo = x.Activo,
                EstadoActivo = x.EstadoActivo,
                Fecha_Creacion = x.Fecha_Creacion,
                Fecha_Modificacion = x.Fecha_Modificacion,
                Id_Creador = x.Id_Creador,
                Id_Modificador = x.Id_Modificador
            };
        }

        public async Task NuevoCorreo(CorreoDTO dto)
        {
            var oCorreo = new Correos
            {
                Id_Persona = dto.Id_Persona,
                Direccion_Correo = dto.Direccion_Correo,
                Id_Tipo_Correo = dto.Id_Tipo_Correo,
                Activo = dto.Activo,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Creador // Al crear, el modificador suele ser el mismo creador inicial
            };

            await _repository.NuevoCorreoAsync(oCorreo);
        }

        public async Task EditarCorreo(CorreoDTO dto)
        {
            var oCorreo = new Correos
            {
                Id_Correo = dto.Id_Correo,
                Id_Persona = dto.Id_Persona,
                Direccion_Correo = dto.Direccion_Correo,
                Id_Tipo_Correo = dto.Id_Tipo_Correo,
                Activo = dto.Activo,
                Id_Modificador = dto.Id_Modificador.GetValueOrDefault()
            };

            await _repository.EditarCorreoAsync(oCorreo);
        }

        public async Task EliminarCorreo(int id, int idModificador)
        {
            await _repository.EliminarCorreoAsync(id, idModificador);
        }
    }
}