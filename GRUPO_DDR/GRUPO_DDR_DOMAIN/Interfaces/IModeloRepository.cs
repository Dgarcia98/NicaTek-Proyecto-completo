using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IModeloRepository
    {
        Task<IEnumerable<Modelo>> ListarModeloAsync();
        Task<IEnumerable<Modelo>> ListarModeloFiltroAsync(string filtro);
        Task NuevaModeloAsync(Modelo oModelo);
        Task EditarModeloAsync(Modelo oModelo);
        Task EliminarModeloAsync(int id);
    }
}