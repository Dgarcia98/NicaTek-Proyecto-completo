using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> ListarClientes();

        Task<IEnumerable<Cliente>> ListarPorFiltro(string filtro);

        Task NuevoCliente(Cliente entidad);

        Task EditarCliente(Cliente entidad);

        Task EliminarCliente(int id, int idModificador);
    }
}