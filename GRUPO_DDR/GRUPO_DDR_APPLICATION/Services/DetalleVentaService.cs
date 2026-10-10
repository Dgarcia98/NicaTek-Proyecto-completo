using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly IDetalleVentaRepository _repository;

        public DetalleVentaService(IDetalleVentaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DetalleVentaDTO>> ListarDetalleVenta()
        {
            var detalles = await _repository.ListarDetalleVentaAsync();
            return detalles.Select(d => new DetalleVentaDTO
            {
                Id_Detalle = d.Id_Detalle,
                Id_Venta = d.Id_Venta,
                Total_Venta_General = d.Total_Venta_General,
                Id_Producto = d.Id_Producto,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                Precio_Unitario_Al_Momento_Venta = d.Precio_Unitario_Al_Momento_Venta,
                Subtotal = d.Subtotal,
                Id_Estado = d.Id_Estado,
                Estado = d.Estado,
                Fecha_Creacion = d.Fecha_Creacion,
                Fecha_Modificacion = d.Fecha_Modificacion,
                Id_Creador = d.Id_Creador,
                Id_Modificador = d.Id_Modificador
            });
        }

        public async Task<DetalleVentaDTO> ObtenerDetalleVentaPorId(int id)
        {
            var detalles = await _repository.ListarDetalleVentaFiltroAsync(id.ToString());
            var d = detalles.FirstOrDefault(x => x.Id_Detalle == id);

            if (d == null) return null;

            return new DetalleVentaDTO
            {
                Id_Detalle = d.Id_Detalle,
                Id_Venta = d.Id_Venta,
                Total_Venta_General = d.Total_Venta_General,
                Id_Producto = d.Id_Producto,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                Precio_Unitario_Al_Momento_Venta = d.Precio_Unitario_Al_Momento_Venta,
                Subtotal = d.Subtotal,
                Id_Estado = d.Id_Estado,
                Estado = d.Estado,
                Fecha_Creacion = d.Fecha_Creacion,
                Fecha_Modificacion = d.Fecha_Modificacion,
                Id_Creador = d.Id_Creador,
                Id_Modificador = d.Id_Modificador
            };
        }

        public async Task<IEnumerable<DetalleVentaDTO>> ListarDetalleVentaFiltro(string filtro)
        {
            var detalles = await _repository.ListarDetalleVentaFiltroAsync(filtro);
            return detalles.Select(d => new DetalleVentaDTO
            {
                Id_Detalle = d.Id_Detalle,
                Id_Venta = d.Id_Venta,
                Total_Venta_General = d.Total_Venta_General,
                Id_Producto = d.Id_Producto,
                Producto = d.Producto,
                Cantidad = d.Cantidad,
                Precio_Unitario_Al_Momento_Venta = d.Precio_Unitario_Al_Momento_Venta,
                Subtotal = d.Subtotal,
                Id_Estado = d.Id_Estado,
                Estado = d.Estado,
                Fecha_Creacion = d.Fecha_Creacion,
                Fecha_Modificacion = d.Fecha_Modificacion,
                Id_Creador = d.Id_Creador,
                Id_Modificador = d.Id_Modificador
            });
        }

        public async Task NuevoDetalleVenta(DetalleVentaDTO dto)
        {
            var entidad = new DetalleVenta
            {
                Id_Venta = dto.Id_Venta,
                Id_Producto = dto.Id_Producto,
                Cantidad = dto.Cantidad,
                Precio_Unitario_Al_Momento_Venta = dto.Precio_Unitario_Al_Momento_Venta,
                Subtotal = dto.Subtotal,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.NuevoDetalleVentaAsync(entidad);
        }

        public async Task EditarDetalleVenta(DetalleVentaDTO dto)
        {
            var entidad = new DetalleVenta
            {
                Id_Detalle = dto.Id_Detalle,
                Id_Producto = dto.Id_Producto,
                Cantidad = dto.Cantidad,
                Precio_Unitario_Al_Momento_Venta = dto.Precio_Unitario_Al_Momento_Venta,
                Subtotal = dto.Subtotal,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.EditarDetalleVentaAsync(entidad);
        }

        public async Task EliminarDetalleVenta(int id, int idModificador)
        {
            await _repository.EliminarDetalleVentaAsync(id, idModificador);
        }
    }
}