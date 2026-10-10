using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ITelefonoRepository
    {
        Task<IEnumerable<Telefonos>> ListarTelefonosAsync();
        Task<IEnumerable<Telefonos>> ListarTelefonosFiltroAsync(string filtro);
        Task NuevoTelefonoAsync(Telefonos oTelefono);
        Task EditarTelefonoAsync(Telefonos oTelefono);
        Task EliminarTelefonoAsync(int id, int idModificador);
    }
}