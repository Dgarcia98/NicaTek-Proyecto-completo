using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class BodegaProductoService : IBodegaProductoService
    {
        private readonly IBodegaProductoRepository _repository;

        public BodegaProductoService(IBodegaProductoRepository repository)
        {
            _repository = repository;
        }

        private BodegaProductoDTO ToDTO(BodegaProducto item) => new BodegaProductoDTO
        {
            Id_Bodega_Producto = item.Id_Bodega_Producto,
            Id_Bodega = item.Id_Bodega,
            Nombre_Bodega = item.Nombre_Bodega,
            Id_Producto = item.Id_Producto,
            Codigo_Producto = item.Codigo_Producto,
            Nombre_Producto = item.Nombre_Producto,
            Precio_Producto = item.Precio_Producto,
            Id_Marca = item.Id_Marca,
            Nombre_Marca = item.Nombre_Marca,
            Id_Modelo = item.Id_Modelo,
            Nombre_Modelo = item.Nombre_Modelo,
            Nombre_Categoria = item.Nombre_Categoria,
            Cantidad = item.Cantidad,
            Estado_Bodega = item.Estado_Bodega,
            Fecha_Creacion = item.Fecha_Creacion,
            Fecha_Modificacion = item.Fecha_Modificacion,
            Id_Creador = item.Id_Creador,
            Id_Modificador = item.Id_Modificador
        };

        public async Task<IEnumerable<BodegaProductoDTO>> ListarBodegaProducto()
        {
            var lista = await _repository.ListarAsync();
            return lista.Select(ToDTO);
        }

        public async Task<IEnumerable<BodegaProductoDTO>> FiltrarBodegaProducto(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return Enumerable.Empty<BodegaProductoDTO>();
            var lista = await _repository.FiltrarAsync(filtro);
            return lista.Select(ToDTO);
        }

        public async Task<IEnumerable<BodegaProductoDTO>> ListarPorBodega(int idBodega)
        {
            var lista = await _repository.ListarPorBodegaAsync(idBodega);
            return lista.Select(ToDTO);
        }

        public async Task NuevoBodegaProducto(BodegaProductoDTO dto)
        {
            await _repository.NuevoAsync(new BodegaProducto
            {
                Id_Bodega = dto.Id_Bodega,
                Id_Producto = dto.Id_Producto,
                Cantidad = dto.Cantidad,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador ?? dto.Id_Creador
            });
        }

        public async Task EditarBodegaProducto(BodegaProductoDTO dto)
        {
            await _repository.EditarAsync(new BodegaProducto
            {
                Id_Bodega_Producto = dto.Id_Bodega_Producto,
                Cantidad = dto.Cantidad,
                Id_Modificador = dto.Id_Modificador ?? dto.Id_Creador
            });
        }

        public async Task EliminarBodegaProducto(int id, int idModificador)
        {
            await _repository.EliminarAsync(id, idModificador);
        }
    }
}