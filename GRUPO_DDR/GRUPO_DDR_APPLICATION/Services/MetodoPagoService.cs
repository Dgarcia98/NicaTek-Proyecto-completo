using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly IMetodoPagoRepository _repository;

        public MetodoPagoService(IMetodoPagoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MetodoPagoDTO>> ListarMetodoPago()
        {
            var listar = await _repository.ListarMetodosPagoAsync();

            return listar.Select(item => new MetodoPagoDTO
            {
                Id_Metodo_Pago = item.Id_Metodo_Pago,
                Descripcion = item.Descripcion,
                Id_Estado = item.Id_Estado,
                Activo = item.Activo,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<IEnumerable<MetodoPagoDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<MetodoPagoDTO>();

            var lista = await _repository.ListarMetodosPagoFiltroAsync(Buscar);

            return lista.Select(item => new MetodoPagoDTO
            {
                Id_Metodo_Pago = item.Id_Metodo_Pago,
                Descripcion = item.Descripcion,
                Id_Estado = item.Id_Estado,
                Activo = item.Activo,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<MetodoPagoDTO> ObtenerMetodoPagoPorId(int id)
        {
            var lista = await _repository.ListarMetodosPagoFiltroAsync(id.ToString());
            var item = lista.FirstOrDefault();

            if (item == null) return null;

            return new MetodoPagoDTO
            {
                Id_Metodo_Pago = item.Id_Metodo_Pago,
                Descripcion = item.Descripcion,
                Id_Estado = item.Id_Estado,
                Activo = item.Activo,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task NuevoMetodoPago(MetodoPagoDTO dto)
        {
            var oMetodoPago = new MetodoPago
            {
                Descripcion = dto.Descripcion,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Creador,
                Activo = true
            };

            await _repository.NuevoMetodoPagoAsync(oMetodoPago);
        }

        public async Task EditarMetodoPago(MetodoPagoDTO dto)
        {
            var oMetodoPago = new MetodoPago
            {
                Id_Metodo_Pago = dto.Id_Metodo_Pago,
                Descripcion = dto.Descripcion,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador,
                Activo = dto.Activo
            };

            await _repository.EditarMetodoPagoAsync(oMetodoPago);
        }

        public async Task EliminarMetodoPago(int id, int idModificador)
        {
            await _repository.EliminarMetodoPagoAsync(id, idModificador);
        }
    }
}