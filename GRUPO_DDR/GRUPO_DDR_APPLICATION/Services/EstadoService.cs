using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class EstadoService : IEstadoService
    {
        private readonly IEstadoRepository _estadoRepository;

        public EstadoService(IEstadoRepository estadoRepository)
        {
            _estadoRepository = estadoRepository;
        }

        public async Task<IEnumerable<EstadoDTO>> ListarEstado()
        {
            var listar = await _estadoRepository.ListarEstadosAsync();

            return listar.Select(item => new EstadoDTO
            {
                Id_Estado = item.Id_Estado,
                Nombre = item.Nombre,
                Estado = item.Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<IEnumerable<EstadoDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<EstadoDTO>();

            var lista = await _estadoRepository.ListarEstadosFiltroAsync(Buscar);

            return lista.Select(item => new EstadoDTO
            {
                Id_Estado = item.Id_Estado,
                Nombre = item.Nombre,
                Estado = item.Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<EstadoDTO> ObtenerEstadoPorId(int id)
        {
            var lista = await _estadoRepository.ListarEstadosFiltroAsync(id.ToString());
            var item = lista.FirstOrDefault();

            if (item == null) return null;

            return new EstadoDTO
            {
                Id_Estado = item.Id_Estado,
                Nombre = item.Nombre,
                Estado = item.Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task NuevoEstado(EstadoDTO dto)
        {
            var oEstado = new Cls_Estados
            {
                Nombre = dto.Nombre,
                Estado = dto.Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Creador
            };

            await _estadoRepository.NuevoEstadoAsync(oEstado);
        }

        public async Task EditarEstado(EstadoDTO dto)
        {
            var oEstado = new Cls_Estados
            {
                Id_Estado = dto.Id_Estado,
                Nombre = dto.Nombre,
                Estado = dto.Estado,
                Id_Modificador = dto.Id_Modificador
            };

            await _estadoRepository.EditarEstadoAsync(oEstado);
        }

        public async Task EliminarEstado(int id, int idModificador)
        {
            await _estadoRepository.EliminarEstadoAsync(id, idModificador);
        }
    }
}