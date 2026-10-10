using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDTO>> ListarProducto();
        Task<ProductoDTO> ObtenerProductoPorId(int id);
        Task<IEnumerable<ProductoDTO>> ListarPorNombre(string Buscar);
        Task NuevoProducto(ProductoDTO dto);
        Task EditarProducto(ProductoDTO dto);
        Task EliminarProducto(int id);
    }
}