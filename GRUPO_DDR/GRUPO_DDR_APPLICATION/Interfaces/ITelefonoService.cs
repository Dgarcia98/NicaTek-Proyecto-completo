using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface ITelefonoService
    {
        Task<IEnumerable<TelefonoDTO>> ListarTelefono();
        Task<TelefonoDTO> ObtenerTelefonoPorId(int id);
        Task<IEnumerable<TelefonoDTO>> ListarPorNumero(string Buscar);
        Task NuevoTelefono(TelefonoDTO dto);
        Task EditarTelefono(TelefonoDTO dto);
        Task EliminarTelefono(int id);
    }
}