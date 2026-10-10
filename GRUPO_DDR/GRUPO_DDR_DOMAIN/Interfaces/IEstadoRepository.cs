using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IEstadoRepository
    {
        Task<IEnumerable<Cls_Estados>> ListarEstadosAsync();
        Task<IEnumerable<Cls_Estados>> ListarEstadosFiltroAsync(string filtro);
        Task NuevoEstadoAsync(Cls_Estados oEstado);
        Task EditarEstadoAsync(Cls_Estados oEstado);
        Task EliminarEstadoAsync(int id, int idModificador); 
    }
}