using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IMarcaModeloRepository
    {
        Task<IEnumerable<MarcaModelo>> ListarMarcaModeloAsync();
        Task<IEnumerable<MarcaModelo>> ListarMarcaModeloFiltroAsync(string filtro);

        Task NuevaMarcaModeloAsync(MarcaModelo entidad);
        Task EditarMarcaModeloAsync(MarcaModelo entidad);
        Task EliminarMarcaModeloAsync(int id);
    }
}