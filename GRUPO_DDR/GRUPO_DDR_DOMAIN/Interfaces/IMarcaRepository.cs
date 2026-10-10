using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IMarcaRepository
    {
        Task<IEnumerable<Marca>> ListarMarcaAsync();
        Task<IEnumerable<Marca>> ListarMarcaFiltroAsync(string filtro);
        Task NuevaMarcaAsync(Marca oMarca);
        Task EditarMarcaAsync(Marca oMarca);
        Task EliminarMarcaAsync(int id);
    }
}