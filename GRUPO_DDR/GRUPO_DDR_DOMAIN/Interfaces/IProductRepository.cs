using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Productos>> ListarProductoAsync();

        Task<IEnumerable<Productos>> ListarProductosFiltroAsync(string filtro);

        Task NuevoProductoAsync(Productos oProducto);

        Task EditarProductoAsync(Productos oProducto);

        Task EliminarProductoAsync(int id);
    }
}   // este es el metodo de lovo
