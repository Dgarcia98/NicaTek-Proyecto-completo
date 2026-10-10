using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class DetalleCompraService : IDetalleCompraService
    {
        private readonly IDetalleCompraRepository _repository;
        private readonly IMovimientoInventarioRepository _movimientoRepository;

        public DetalleCompraService(
            IDetalleCompraRepository repository,
            IMovimientoInventarioRepository movimientoRepository)
        {
            _repository = repository;
            _movimientoRepository = movimientoRepository;
        }

        public async Task<IEnumerable<DetalleCompraDTO>> ListarDetalleCompra()
        {
            var detalles = await _repository.ListarDetalleCompraAsync();
            return detalles.Select(d => new DetalleCompraDTO
            {
                Id_Detalle_Compra = d.Id_Detalle_Compra,
                Id_Compra = d.Id_Compra,
                Total_Compra = d.Total_Compra,
                Id_Producto = d.Id_Producto,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                Precio_Unitario_Compra = d.Precio_Unitario_Compra,
                Subtotal_Compra = d.Subtotal_Compra,
                Fecha_Creacion = d.Fecha_Creacion,
                Fecha_Modificacion = d.Fecha_Modificacion,
                Id_Creador = d.Id_Creador,
                Id_Modificador = d.Id_Modificador
            });
        }

        public async Task<DetalleCompraDTO> ObtenerDetalleCompraPorId(int id)
        {
            var detalles = await _repository.FiltrarDetalleCompraAsync(id.ToString());
            var d = detalles.FirstOrDefault(x => x.Id_Detalle_Compra == id);

            if (d == null) return null;

            return new DetalleCompraDTO
            {
                Id_Detalle_Compra = d.Id_Detalle_Compra,
                Id_Compra = d.Id_Compra,
                Total_Compra = d.Total_Compra,
                Id_Producto = d.Id_Producto,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                Precio_Unitario_Compra = d.Precio_Unitario_Compra,
                Subtotal_Compra = d.Subtotal_Compra,
                Fecha_Creacion = d.Fecha_Creacion,
                Fecha_Modificacion = d.Fecha_Modificacion,
                Id_Creador = d.Id_Creador,
                Id_Modificador = d.Id_Modificador
            };
        }

        public async Task<IEnumerable<DetalleCompraDTO>> ListarPorFiltro(string Buscar)
        {
            var detalles = await _repository.FiltrarDetalleCompraAsync(Buscar);
            return detalles.Select(d => new DetalleCompraDTO
            {
                Id_Detalle_Compra = d.Id_Detalle_Compra,
                Id_Compra = d.Id_Compra,
                Total_Compra = d.Total_Compra,
                Id_Producto = d.Id_Producto,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                Precio_Unitario_Compra = d.Precio_Unitario_Compra,
                Subtotal_Compra = d.Subtotal_Compra,
                Fecha_Creacion = d.Fecha_Creacion,
                Fecha_Modificacion = d.Fecha_Modificacion,
                Id_Creador = d.Id_Creador,
                Id_Modificador = d.Id_Modificador
            });
        }

        public async Task NuevoDetalleCompra(DetalleCompraDTO dto)
        {
            // Solo mandamos a llamar al Repositorio. 
            // ¡El Stored Procedure se encargará de hacer la magia del Kardex en SQL!
            var entidad = new DetalleCompra
            {
                Id_Compra = dto.Id_Compra,
                Id_Producto = dto.Id_Producto,
                Id_Bodega = dto.Id_Bodega,
                Cantidad = dto.Cantidad,
                Precio_Unitario_Compra = dto.Precio_Unitario_Compra,
                Subtotal_Compra = dto.Subtotal_Compra,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.NuevoDetalleCompraAsync(entidad);
        }

        public async Task EditarDetalleCompra(DetalleCompraDTO dto)
        {
            var entidad = new DetalleCompra
            {
                Id_Detalle_Compra = dto.Id_Detalle_Compra,
                Cantidad = dto.Cantidad,
                Precio_Unitario_Compra = dto.Precio_Unitario_Compra,
                Subtotal_Compra = dto.Subtotal_Compra,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.EditarDetalleCompraAsync(entidad);
        }

        public async Task EliminarDetalleCompra(int id, int idModificador)
        {
            await _repository.EliminarDetalleCompraAsync(id, idModificador);
        }
    }
}