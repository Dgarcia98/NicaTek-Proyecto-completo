using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IDireccionRepository
    {
        Task<IEnumerable<Direccion>> ListarDireccionesAsync();
        Task<IEnumerable<Direccion>> FiltrarDireccionesAsync(string filtro);

        Task NuevaDireccionAsync(Direccion direccion);
        Task EditarDireccionAsync(Direccion direccion);
        Task EliminarDireccionAsync(int id, int idModificador);
    }
}