using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ICatalogoRepository
    {
        Task<IEnumerable<Catalogo>> ListarCatalogosAsync();
        Task<IEnumerable<Catalogo>> ListarCatalogosFiltroAsync(string filtro);
        Task NuevoCatalogoAsync(Catalogo catalogo);
        Task EditarCatalogoAsync(Catalogo catalogo);
        Task EliminarCatalogoAsync(int id, int idModificador);
    }
}