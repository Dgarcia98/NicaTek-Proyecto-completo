using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> Login(LoginDTO dto);

        Task<IEnumerable<UsuarioDTO>> ListarUsuarios();

        Task<UsuarioDTO> ObtenerUsuarioPorId(int id);

        Task<IEnumerable<UsuarioDTO>> ListarPorFiltro(string filtro);

        Task NuevoUsuario(UsuarioDTO dto);

        Task EditarUsuario(UsuarioDTO dto);

        Task EliminarUsuario(int id, int idModificador);
    }
}