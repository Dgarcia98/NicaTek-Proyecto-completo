using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class BodegaService : IBodegaService
    {
        private readonly IBodegaRepository _repository;

        public BodegaService(IBodegaRepository repository)
        {
            _repository = repository;
        }

        private BodegaDTO ToDTO(Bodegas item) => new BodegaDTO
        {
            Id_Bodega = item.Id_Bodega,
            Nombre_Bodega = item.Nombre_Bodega,
            Cantidad = item.Cantidad,
            Total_Productos = item.Total_Productos,
            Id_Estado = item.Id_Estado,
            Nombre_Estado = item.Nombre_Estado,
            Fecha_Creacion = item.Fecha_Creacion,
            Fecha_Modificacion = item.Fecha_Modificacion,
            Id_Creador = item.Id_Creador,
            Id_Modificador = item.Id_Modificador
        };

        public async Task<IEnumerable<BodegaDTO>> ListarBodega()
        {
            var lista = await _repository.ListarBodegaAsync();
            return lista.Select(ToDTO);
        }

        public async Task<IEnumerable<BodegaDTO>> ListarPorNombre(string buscar)
        {
            if (string.IsNullOrWhiteSpace(buscar))
                return Enumerable.Empty<BodegaDTO>();
            var lista = await _repository.ListarBodegaFiltroAsync(buscar);
            return lista.Select(ToDTO);
        }

        public async Task<BodegaDTO> ObtenerBodegaPorId(int id)
        {
            var lista = await _repository.ListarBodegaFiltroAsync(id.ToString());
            var item = lista.FirstOrDefault(x => x.Id_Bodega == id);
            return item == null ? null : ToDTO(item);
        }

        public async Task NuevoBodega(BodegaDTO dto)
        {
            await _repository.NuevaBodegaAsync(new Bodegas
            {
                Nombre_Bodega = dto.Nombre_Bodega,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador ?? dto.Id_Creador
            });
        }

        public async Task EditarBodega(BodegaDTO dto)
        {
            await _repository.EditarBodegaAsync(new Bodegas
            {
                Id_Bodega = dto.Id_Bodega,
                Nombre_Bodega = dto.Nombre_Bodega,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador ?? dto.Id_Creador
            });
        }

        public async Task EliminarBodega(int id, int idModificador)
        {
            await _repository.EliminarBodegaAsync(id, idModificador);
        }
    }
}