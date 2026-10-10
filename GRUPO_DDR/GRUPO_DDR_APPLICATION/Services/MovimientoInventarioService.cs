using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {
        private readonly IMovimientoInventarioRepository _repository;

        public MovimientoInventarioService(IMovimientoInventarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MovimientoInventarioDTO>> ListarMovimientos()
        {
            var movimientos = await _repository.ListarMovimientosAsync();
            return movimientos.Select(m => new MovimientoInventarioDTO
            {
                Id_Movimiento = m.Id_Movimiento,
                Fecha_Movimiento = m.Fecha_Movimiento,
                Id_Producto = m.Id_Producto,
                Producto = m.Producto,
                Id_Tipo_Movimiento = m.Id_Tipo_Movimiento,
                Tipo_Movimiento = m.Tipo_Movimiento,
                Cantidad = m.Cantidad,
                Stock_Anterior = m.Stock_Anterior,
                Stock_Actual = m.Stock_Actual,
                Id_Bodega = m.Id_Bodega,
                Costo_Unitario = m.Costo_Unitario,
                Referencia = m.Referencia,
                Observaciones = m.Observaciones,
                Id_Usuario_Responsable = m.Id_Usuario_Responsable,
                Responsable = m.Responsable
            });
        }

        public async Task<IEnumerable<MovimientoInventarioDTO>> ListarMovimientosFiltro(string filtro)
        {
            var movimientos = await _repository.ListarMovimientosFiltroAsync(filtro);
            return movimientos.Select(m => new MovimientoInventarioDTO
            {
                Id_Movimiento = m.Id_Movimiento,
                Fecha_Movimiento = m.Fecha_Movimiento,
                Id_Producto = m.Id_Producto,
                Producto = m.Producto,
                Id_Tipo_Movimiento = m.Id_Tipo_Movimiento,
                Tipo_Movimiento = m.Tipo_Movimiento,
                Cantidad = m.Cantidad,
                Stock_Anterior = m.Stock_Anterior,
                Stock_Actual = m.Stock_Actual,
                Id_Bodega = m.Id_Bodega,
                Costo_Unitario = m.Costo_Unitario,
                Referencia = m.Referencia,
                Observaciones = m.Observaciones,
                Id_Usuario_Responsable = m.Id_Usuario_Responsable,
                Responsable = m.Responsable
            });
        }

        public async Task NuevoMovimiento(MovimientoInventarioDTO dto)
        {
            var entidad = new MovimientoInventario
            {
                Id_Producto = dto.Id_Producto,
                Id_Bodega = dto.Id_Bodega,
                Id_Tipo_Movimiento = dto.Id_Tipo_Movimiento,
                Cantidad = dto.Cantidad,
                Costo_Unitario = dto.Costo_Unitario,
                Referencia = dto.Referencia,
                Observaciones = dto.Observaciones,
                Id_Usuario_Responsable = dto.Id_Usuario_Responsable
            };

            await _repository.NuevoMovimientoAsync(entidad);
        }
    }
}