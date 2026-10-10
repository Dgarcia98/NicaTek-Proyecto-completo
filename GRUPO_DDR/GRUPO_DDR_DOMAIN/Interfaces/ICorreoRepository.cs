using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ICorreoRepository
    {
        Task<IEnumerable<Correos>> ListarCorreoAsync();
        Task<IEnumerable<Correos>> ListarCorreoFiltroAsync(string filtro);
        Task NuevoCorreoAsync(Correos oCorreo);
        Task EditarCorreoAsync(Correos oCorreo);
        Task EliminarCorreoAsync(int id, int idModificador);
    }
}