using GRUPO_DDR_API.Authorization;
using GRUPO_DDR_API.Middleware;
using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly IUsuarioActual _usuarioActual;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(
            IUsuarioService service,
            IUsuarioActual usuarioActual,
            ILogger<UsuarioController> logger)
        {
            _service = service;
            _usuarioActual = usuarioActual;
            _logger = logger;
        }

        /// <summary>
        /// Unico endpoint publico de la API. Todo lo demas exige token por
        /// el filtro global de Program.cs.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { exito = false, mensaje = "Datos de inicio de sesion incompletos." });

            var usuario = await _service.Login(dto);

            if (usuario == null)
            {
                // Mensaje unico para usuario inexistente y contrasena
                // incorrecta: distinguirlos permite enumerar usuarios.
                _logger.LogWarning("Intento de login fallido para '{Usuario}' desde {IP}",
                    dto?.Nombre_Usuario, HttpContext.Connection.RemoteIpAddress);

                return Unauthorized(new { exito = false, mensaje = "Usuario o contrasena incorrectos." });
            }

            return Ok(usuario);
        }

        /// <summary>
        /// Datos del usuario de la sesion actual. El frontend lo usa para
        /// rehidratar la sesion al recargar sin volver a pedir credenciales.
        /// </summary>
        [HttpGet("Yo")]
        public IActionResult Yo()
        {
            return Ok(new
            {
                id_Usuario = _usuarioActual.IdUsuario,
                nombre_Usuario = _usuarioActual.NombreUsuario,
                nombre_Rol = _usuarioActual.NombreRol,
                permisos = User.Claims
                    .Where(c => c.Type == PermisoConstantes.ClaimTipo)
                    .Select(c => c.Value)
                    .ToList()
            });
        }

        [RequierePermiso("USR_MANAGE")]
        [HttpGet("ListarUsuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var lista = await _service.ListarUsuarios();
            return Ok(lista);
        }

        [RequierePermiso("USR_MANAGE")]
        [HttpGet("ObtenerUsuarioPorId/{id}")]
        public async Task<IActionResult> ObtenerUsuarioPorId(int id)
        {
            var item = await _service.ObtenerUsuarioPorId(id);
            if (item == null)
                return NotFound(new { exito = false, mensaje = $"No se encontro el usuario con ID {id}" });

            return Ok(item);
        }

        [RequierePermiso("USR_MANAGE")]
        [HttpGet("ListarPorFiltro/{Buscar}")]
        public async Task<IActionResult> ListarPorFiltro(string Buscar)
        {
            var lista = await _service.ListarPorFiltro(Buscar);
            return Ok(lista);
        }

        [RequierePermiso("USR_MANAGE")]
        [HttpPost("NuevoUsuario")]
        public async Task<IActionResult> NuevoUsuario([FromBody] UsuarioDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { exito = false, mensaje = "El modelo no es valido." });

            if (string.IsNullOrWhiteSpace(dto.Contrasena) || dto.Contrasena.Length < 8)
                throw new ReglaNegocioException("La contrasena debe tener al menos 8 caracteres.");

            // El creador sale del token, no del cuerpo del request.
            dto.Id_Creador = _usuarioActual.IdUsuarioRequerido;

            await _service.NuevoUsuario(dto);
            return StatusCode(201, new { exito = true, mensaje = "Usuario registrado correctamente." });
        }

        [RequierePermiso("USR_MANAGE")]
        [HttpPut("EditarUsuario/{id}")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] UsuarioDTO dto)
        {
            if (id != dto.Id_Usuario)
                return BadRequest(new { exito = false, mensaje = "El ID de la URL no coincide con el del cuerpo." });

            if (!string.IsNullOrWhiteSpace(dto.Contrasena) && dto.Contrasena.Length < 8)
                throw new ReglaNegocioException("La contrasena debe tener al menos 8 caracteres.");

            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            await _service.EditarUsuario(dto);
            return Ok(new { exito = true, mensaje = "Usuario actualizado correctamente" });
        }

        [RequierePermiso("USR_MANAGE")]
        [HttpDelete("EliminarUsuario/{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var idModificador = _usuarioActual.IdUsuarioRequerido;

            if (id == idModificador)
                throw new ReglaNegocioException("No puede desactivar su propio usuario.");

            var item = await _service.ObtenerUsuarioPorId(id);
            if (item == null)
                return NotFound(new { exito = false, mensaje = "El usuario que intenta eliminar no existe." });

            await _service.EliminarUsuario(id, idModificador);
            return Ok(new { exito = true, mensaje = "Usuario inactivado correctamente" });
        }

        /// <summary>
        /// Compatibilidad con el frontend anterior, que mandaba el
        /// idModificador en la URL. Se ignora ese valor y se usa el del
        /// token. Eliminar cuando el frontend este desplegado.
        /// </summary>
        [RequierePermiso("USR_MANAGE")]
        [HttpDelete("EliminarUsuario/{id}/{idModificador}")]
        public Task<IActionResult> EliminarUsuarioLegacy(int id, int idModificador)
            => EliminarUsuario(id);
    }
}
