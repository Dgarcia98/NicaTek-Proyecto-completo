using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _repository;

        public CompraService(ICompraRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResultadoRegistroDTO> RegistrarCompra(CompraRegistroDTO dto, int idUsuario)
        {
            var detalles = dto.Detalles
                .Select(d => (d.Id_Producto, d.Id_Bodega, d.Cantidad, d.Precio_Unitario_Compra));

            var (id, mensaje) = await _repository.RegistrarCompraAsync(
                dto.Id_Proveedor,
                dto.Numero_Factura_Proveedor,
                dto.Fecha_Compra,
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

        public async Task<IEnumerable<CompraDTO>> ListarCompra()
        {
            var compras = await _repository.ListarCompraAsync();
            return compras.Select(c => new CompraDTO
            {
                Id_Compra = c.Id_Compra,
                Id_Proveedor = c.Id_Proveedor,
                Proveedor = c.Proveedor,
                Numero_Factura_Proveedor = c.Numero_Factura_Proveedor,
                Fecha_Compra = c.Fecha_Compra,
                Total_Compra = c.Total_Compra,
                Id_Estado = c.Id_Estado,
                Estado = c.Estado,
                Fecha_Creacion = c.Fecha_Creacion,
                Fecha_Modificacion = c.Fecha_Modificacion,
                Id_Creador = c.Id_Creador,
                Id_Modificador = c.Id_Modificador
            });
        }

        public async Task<CompraDTO> ObtenerCompraPorId(int id)
        {
            var compras = await _repository.FiltrarCompraAsync(id.ToString());
            var c = compras.FirstOrDefault(x => x.Id_Compra == id);

            if (c == null) return null;

            return new CompraDTO
            {
                Id_Compra = c.Id_Compra,
                Id_Proveedor = c.Id_Proveedor,
                Proveedor = c.Proveedor,
                Numero_Factura_Proveedor = c.Numero_Factura_Proveedor,
                Fecha_Compra = c.Fecha_Compra,
                Total_Compra = c.Total_Compra,
                Id_Estado = c.Id_Estado,
                Estado = c.Estado,
                Fecha_Creacion = c.Fecha_Creacion,
                Fecha_Modificacion = c.Fecha_Modificacion,
                Id_Creador = c.Id_Creador,
                Id_Modificador = c.Id_Modificador
            };
        }

        public async Task<IEnumerable<CompraDTO>> ListarPorFiltro(string Buscar)
        {
            var compras = await _repository.FiltrarCompraAsync(Buscar);
            return compras.Select(c => new CompraDTO
            {
                Id_Compra = c.Id_Compra,
                Id_Proveedor = c.Id_Proveedor,
                Proveedor = c.Proveedor,
                Numero_Factura_Proveedor = c.Numero_Factura_Proveedor,
                Fecha_Compra = c.Fecha_Compra,
                Total_Compra = c.Total_Compra,
                Id_Estado = c.Id_Estado,
                Estado = c.Estado,
                Fecha_Creacion = c.Fecha_Creacion,
                Fecha_Modificacion = c.Fecha_Modificacion,
                Id_Creador = c.Id_Creador,
                Id_Modificador = c.Id_Modificador
            });
        }

        public async Task<int> NuevoCompra(CompraDTO dto)
        {
            var entidad = new Compra
            {
                Id_Proveedor = dto.Id_Proveedor,
                Numero_Factura_Proveedor = dto.Numero_Factura_Proveedor,
                Fecha_Compra = dto.Fecha_Compra,  // ✅ FIX 2: campo agregado
                Total_Compra = dto.Total_Compra,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };

            int idGenerado = await _repository.NuevaCompraAsync(entidad);
            return idGenerado;
        }

        public async Task EditarCompra(CompraDTO dto)
        {
            var entidad = new Compra
            {
                Id_Compra = dto.Id_Compra,
                Id_Proveedor = dto.Id_Proveedor,
                Numero_Factura_Proveedor = dto.Numero_Factura_Proveedor,
                Fecha_Compra = dto.Fecha_Compra,
                Total_Compra = dto.Total_Compra,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.EditarCompraAsync(entidad);
        }

        public async Task EliminarCompra(int id, int idModificador)
        {
            await _repository.EliminarCompraAsync(id, idModificador);
        }
    }
}