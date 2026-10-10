using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Login(string usuario);

        /// <summary>
        /// Codigos de permiso efectivos del usuario (via su rol).
        /// Se emiten como claims dentro del JWT para que la API pueda
        /// autorizar sin consultar la base en cada request.
        /// </summary>
        Task<IEnumerable<string>> ObtenerPermisos(int idUsuario);

        Task<IEnumerable<Usuario>> ListarUsuarios();

        Task<IEnumerable<Usuario>> ListarPorFiltro(string filtro);
        Task NuevoUsuario(Usuario entidad);

        Task EditarUsuario(Usuario entidad);

        Task EliminarUsuario(int id, int idModificador);
    }
}