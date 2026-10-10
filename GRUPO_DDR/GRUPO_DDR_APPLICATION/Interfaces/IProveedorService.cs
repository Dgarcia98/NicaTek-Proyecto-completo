using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IProveedorService
    {
        Task<IEnumerable<ProveedorDTO>> ListarProveedor();
        Task<IEnumerable<ProveedorDTO>> ListarPorNombre(string Buscar);
        Task<ProveedorDTO> ObtenerProveedorPorId(int id);
        Task NuevoProveedor(ProveedorDTO dto);
        Task EditarProveedor(ProveedorDTO dto);
        Task EliminarProveedor(int id);
    }
}