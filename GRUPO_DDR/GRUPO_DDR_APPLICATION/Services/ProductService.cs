using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<ProductoDTO>> ListarProducto()
        {
            var listar = await _productoRepository.ListarProductoAsync();

            return listar.Select(p => new ProductoDTO
            {
                Id_Producto = p.Id_Producto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Precio_Compra = p.Precio_Compra,
                Codigo = p.Codigo,
                Id_Marca_Modelo = p.Id_Marca_Modelo,
                Id_Proveedor = p.Id_Proveedor ?? 0,
                Id_Categoria = p.Id_Categoria ?? 0,
                Id_Estado = p.Id_Estado,
                Id_Creador = p.Id_Creador,
                Id_Modificador = p.Id_Modificador ?? 0,
                Nombre_Marca = p.Nombre_Marca,
                Nombre_Modelo = p.Nombre_Modelo,
                Proveedor = p.Nombre_Proveedor
            });
        }

        public async Task<IEnumerable<ProductoDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<ProductoDTO>();

            var lista = await _productoRepository.ListarProductosFiltroAsync(Buscar);

            return lista.Select(p => new ProductoDTO
            {
                Id_Producto = p.Id_Producto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Precio_Compra = p.Precio_Compra,
                Codigo = p.Codigo,
                Id_Marca_Modelo = p.Id_Marca_Modelo,
                Id_Proveedor = p.Id_Proveedor ?? 0,
                Id_Categoria = p.Id_Categoria ?? 0,
                Id_Estado = p.Id_Estado,
                Id_Creador = p.Id_Creador,
                Id_Modificador = p.Id_Modificador ?? 0,
                Nombre_Marca = p.Nombre_Marca,
                Nombre_Modelo = p.Nombre_Modelo,
                Proveedor = p.Nombre_Proveedor
            });
        }

        public async Task<ProductoDTO> ObtenerProductoPorId(int id)
        {
            var lista = await _productoRepository.ListarProductosFiltroAsync(id.ToString());
            var p = lista.FirstOrDefault(x => x.Id_Producto == id);

            if (p == null) return null;

            return new ProductoDTO
            {
                Id_Producto = p.Id_Producto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                Precio_Compra = p.Precio_Compra,
                Codigo = p.Codigo,
                Id_Marca_Modelo = p.Id_Marca_Modelo,
                Id_Proveedor = p.Id_Proveedor ?? 0,
                Id_Categoria = p.Id_Categoria ?? 0,
                Id_Estado = p.Id_Estado,
                Id_Creador = p.Id_Creador,
                Id_Modificador = p.Id_Modificador ?? 0,
                Nombre_Marca = p.Nombre_Marca,
                Nombre_Modelo = p.Nombre_Modelo,
                Proveedor = p.Nombre_Proveedor
            };
        }

        public async Task NuevoProducto(ProductoDTO dto)
        {
            var oProducto = new Productos
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Precio_Compra = dto.Precio_Compra,
                Id_Marca_Modelo = dto.Id_Marca_Modelo,
                Id_Proveedor = dto.Id_Proveedor,
                Id_Categoria = dto.Id_Categoria,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador > 0 ? dto.Id_Creador : 1,
                Id_Modificador = dto.Id_Creador > 0 ? dto.Id_Creador : 1
            };

            await _productoRepository.NuevoProductoAsync(oProducto);
        }

        public async Task EditarProducto(ProductoDTO dto)
        {
            var oProducto = new Productos
            {
                Id_Producto = dto.Id_Producto,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Precio_Compra = dto.Precio_Compra,
                Codigo = dto.Codigo,
                Id_Marca_Modelo = dto.Id_Marca_Modelo,
                Id_Proveedor = dto.Id_Proveedor,
                Id_Categoria = dto.Id_Categoria,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador > 0 ? dto.Id_Modificador : 1
            };

            await _productoRepository.EditarProductoAsync(oProducto);
        }

        public async Task EliminarProducto(int id)
        {
            await _productoRepository.EliminarProductoAsync(id);
        }
    }
}