using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ListarCategoriasAsync();
        Task<IEnumerable<Categoria>> ListarCategoriasFiltroAsync(string filtro);
        Task NuevoCategoriaAsync(Categoria categoria);
        Task EditarCategoriaAsync(Categoria categoria);
        Task EliminarCategoriaAsync(int id, int idModificador);
    }
}