using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> ListarClientes();

        Task<ClienteDTO> ObtenerClientePorId(int id);

        Task<IEnumerable<ClienteDTO>> ListarPorFiltro(string Buscar);

        Task NuevoCliente(ClienteDTO dto);

        Task EditarCliente(ClienteDTO dto);

        Task EliminarCliente(int id, int idModificador);
    }
}