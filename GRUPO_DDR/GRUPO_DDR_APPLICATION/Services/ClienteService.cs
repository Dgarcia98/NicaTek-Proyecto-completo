using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<ClienteDTO>> ListarClientes()
        {
            var lista = await _clienteRepository.ListarClientes();
            return lista.Select(item => new ClienteDTO
            {
                Id_Cliente = item.Id_Cliente,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Id_Creador = item.Id_Creador,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Modificador = item.Id_Modificador,
                Estado = item.Estado
            });
        }

        public async Task<ClienteDTO> ObtenerClientePorId(int id)
        {
            var lista = await _clienteRepository.ListarPorFiltro(id.ToString());
            var item = lista.FirstOrDefault(x => x.Id_Cliente == id);

            if (item == null) return null;

            return new ClienteDTO
            {
                Id_Cliente = item.Id_Cliente,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Id_Creador = item.Id_Creador,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Modificador = item.Id_Modificador,
                Estado = item.Estado
            };
        }

        public async Task<IEnumerable<ClienteDTO>> ListarPorFiltro(string Buscar)
        {
            var lista = await _clienteRepository.ListarPorFiltro(Buscar);
            return lista.Select(item => new ClienteDTO
            {
                Id_Cliente = item.Id_Cliente,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Id_Creador = item.Id_Creador,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Modificador = item.Id_Modificador,
                Estado = item.Estado
            });
        }

        public async Task NuevoCliente(ClienteDTO dto)
        {
            var entidad = new Cliente
            {
                Id_Persona = dto.Id_Persona,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };
            await _clienteRepository.NuevoCliente(entidad);
        }

        public async Task EditarCliente(ClienteDTO dto)
        {
            var entidad = new Cliente
            {
                Id_Cliente = dto.Id_Cliente,
                Id_Persona = dto.Id_Persona,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };
            await _clienteRepository.EditarCliente(entidad);
        }

        public async Task EliminarCliente(int id, int idModificador)
        {
            await _clienteRepository.EliminarCliente(id, idModificador);
        }
    }
}