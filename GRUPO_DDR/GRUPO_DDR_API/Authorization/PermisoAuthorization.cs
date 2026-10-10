using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace GRUPO_DDR_API.Authorization
{
    /// <summary>
    /// Autorizacion por permiso concreto (INV_CREATE, POS_SALE, ...).
    ///
    /// Antes los permisos vivian SOLO en el sessionStorage del navegador y
    /// la UI se limitaba a poner 'pointer-events-none' en los botones. Eso
    /// no es seguridad: bastaba con abrir DevTools y reescribir el arreglo.
    ///
    /// Ahora los codigos de permiso viajan como claims firmados dentro del
    /// JWT, se emiten desde la base al hacer login, y se verifican aqui en
    /// cada request. El navegador ya no decide nada.
    /// </summary>

    public class PermisoRequirement : IAuthorizationRequirement
    {
        public string Codigo { get; }
        public PermisoRequirement(string codigo) => Codigo = codigo;
    }

    public class PermisoHandler : AuthorizationHandler<PermisoRequirement>
    {
        /// <summary>
        /// Rol que siempre pasa. Existe como red de seguridad: si la tabla
        /// Tbl_Roles_Permisos queda incompleta, el administrador no se
        /// autobloquea del sistema. Cualquier otro rol necesita el permiso
        /// explicito en base de datos.
        /// </summary>
        public const string RolAdministrador = "Administrador";

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermisoRequirement requirement)
        {
            var usuario = context.User;

            if (usuario?.Identity?.IsAuthenticated != true)
                return Task.CompletedTask;

            if (usuario.IsInRole(RolAdministrador))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var tienePermiso = usuario.Claims.Any(c =>
                c.Type == PermisoConstantes.ClaimTipo &&
                string.Equals(c.Value, requirement.Codigo, StringComparison.OrdinalIgnoreCase));

            if (tienePermiso)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public static class PermisoConstantes
    {
        public const string ClaimTipo = "permiso";
        public const string PrefijoPolitica = "Permiso:";
    }

    /// <summary>
    /// Crea las politicas al vuelo. Sin esto habria que registrar a mano
    /// cada codigo de permiso en Program.cs y mantener dos listas.
    /// </summary>
    public class PermisoPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallback;

        public PermisoPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallback = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallback.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PermisoConstantes.PrefijoPolitica, StringComparison.OrdinalIgnoreCase))
            {
                var codigo = policyName.Substring(PermisoConstantes.PrefijoPolitica.Length);

                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermisoRequirement(codigo))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return _fallback.GetPolicyAsync(policyName);
        }
    }

    /// <summary>
    /// Uso: [RequierePermiso("INV_DELETE")] sobre la accion del controlador.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequierePermisoAttribute : AuthorizeAttribute
    {
        public RequierePermisoAttribute(string codigoPermiso)
        {
            Policy = PermisoConstantes.PrefijoPolitica + codigoPermiso;
        }
    }
}
