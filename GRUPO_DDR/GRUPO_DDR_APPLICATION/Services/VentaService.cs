using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _repository;

        public VentaService(IVentaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VentaDTO>> ListarVentas()
        {
            var ventas = await _repository.ListarVentasAsync();
            return ventas.Select(v => new VentaDTO
            {
                Id_Venta = v.Id_Venta,
                Fecha = v.Fecha,
                Id_Cliente = v.Id_Cliente,
                Cliente = v.Cliente,
                Id_Empleado = v.Id_Empleado,
                Empleado = v.Empleado,
                Id_Metodo_Pago = v.Id_Metodo_Pago,
                Metodo_Pago = v.Metodo_Pago,
                Total = v.Total,
                Id_Estado = v.Id_Estado,
                Estado = v.Estado,
                Fecha_Creacion = v.Fecha_Creacion,
                Fecha_Modificacion = v.Fecha_Modificacion,
                Id_Creador = v.Id_Creador,
                Id_Modificador = v.Id_Modificador
            });
        }

        public async Task<VentaDTO> ObtenerVentaPorId(int id)
        {
            var ventas = await _repository.ListarVentasFiltroAsync(id.ToString());
            var v = ventas.FirstOrDefault(x => x.Id_Venta == id);

            if (v == null) return null;

            return new VentaDTO
            {
                Id_Venta = v.Id_Venta,
                Fecha = v.Fecha,
                Id_Cliente = v.Id_Cliente,
                Cliente = v.Cliente,
                Id_Empleado = v.Id_Empleado,
                Empleado = v.Empleado,
                Id_Metodo_Pago = v.Id_Metodo_Pago,
                Metodo_Pago = v.Metodo_Pago,
                Total = v.Total,
                Id_Estado = v.Id_Estado,
                Estado = v.Estado,
                Fecha_Creacion = v.Fecha_Creacion,
                Fecha_Modificacion = v.Fecha_Modificacion,
                Id_Creador = v.Id_Creador,
                Id_Modificador = v.Id_Modificador
            };
        }

        public async Task<IEnumerable<VentaDTO>> ListarVentasFiltro(string Buscar)
        {
            var ventas = await _repository.ListarVentasFiltroAsync(Buscar);
            return ventas.Select(v => new VentaDTO
            {
                Id_Venta = v.Id_Venta,
                Fecha = v.Fecha,
                Id_Cliente = v.Id_Cliente,
                Cliente = v.Cliente,
                Id_Empleado = v.Id_Empleado,
                Empleado = v.Empleado,
                Id_Metodo_Pago = v.Id_Metodo_Pago,
                Metodo_Pago = v.Metodo_Pago,
                Total = v.Total,
                Id_Estado = v.Id_Estado,
                Estado = v.Estado,
                Fecha_Creacion = v.Fecha_Creacion,
                Fecha_Modificacion = v.Fecha_Modificacion,
                Id_Creador = v.Id_Creador,
                Id_Modificador = v.Id_Modificador
            });
        }

        public async Task<ResultadoRegistroDTO> RegistrarVenta(VentaRegistroDTO dto, int idUsuario)
        {
            var detalles = dto.Detalles
                .Select(d => (d.Id_Producto, d.Id_Bodega, d.Cantidad));

            var (id, mensaje) = await _repository.RegistrarVentaAsync(
                dto.Id_Cliente,
                dto.Id_Metodo_Pago,
                dto.Fecha,
                dto.Id_Estado,
                idUsuario,
                detalles);

            return new ResultadoRegistroDTO
            {
                Exito = id > 0,
                Id = id > 0 ? id : 0,
                Mensaje = mensaje
            };
        }

        public async Task<int> NuevaVenta(VentaDTO dto)
        {
            var entidad = new Venta
            {
                Fecha = dto.Fecha,
                Id_Cliente = dto.Id_Cliente,
                Id_Empleado = dto.Id_Empleado,
                Id_Metodo_Pago = dto.Id_Metodo_Pago,
                Total = dto.Total,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };

            // Retornamos el ID al controlador
            return await _repository.NuevaVentaAsync(entidad);
        }
        public async Task EditarVenta(VentaDTO dto)
        {
            var entidad = new Venta
            {
                Id_Venta = dto.Id_Venta,
                Fecha = dto.Fecha,
                Id_Cliente = dto.Id_Cliente,
                Id_Empleado = dto.Id_Empleado,
                Id_Metodo_Pago = dto.Id_Metodo_Pago,
                Total = dto.Total,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.EditarVentaAsync(entidad);
        }

        public async Task EliminarVenta(int id, int idModificador)
        {
            await _repository.EliminarVentaAsync(id, idModificador);
        }
    }
}