using System.Security.Claims;
using GRUPO_DDR_API.Authorization;
using GRUPO_DDR_APPLICATION.Interfaces;

namespace GRUPO_DDR_API.Services
{
    /// <summary>
    /// Lee la identidad del usuario desde los claims del JWT validado.
    /// Ver <see cref="IUsuarioActual"/> para el porque.
    /// </summary>
    public class UsuarioActual : IUsuarioActual
    {
        private readonly IHttpContextAccessor _accessor;

        public UsuarioActual(IHttpContextAccessor accessor) => _accessor = accessor;

        private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

        public bool EstaAutenticado => Principal?.Identity?.IsAuthenticated == true;

        public int? IdUsuario
        {
            get
            {
                var valor = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(valor, out var id) ? id : null;
            }
        }

        public int IdUsuarioRequerido =>
            IdUsuario ?? throw new UnauthorizedAccessException(
                "La peticion no trae un usuario autenticado valido.");

        public string? NombreUsuario => Principal?.FindFirst(ClaimTypes.Name)?.Value;

        public string? NombreRol => Principal?.FindFirst(ClaimTypes.Role)?.Value;

        public bool TienePermiso(string codigo)
        {
            if (Principal is null) return false;

            if (Principal.IsInRole(PermisoHandler.RolAdministrador)) return true;

            return Principal.Claims.Any(c =>
                c.Type == PermisoConstantes.ClaimTipo &&
                string.Equals(c.Value, codigo, StringComparison.OrdinalIgnoreCase));
        }
    }
}
