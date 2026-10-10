using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolPermisoController : ControllerBase
    {
        private readonly IRolPermisoService _rolPermisoService;

        public RolPermisoController(IRolPermisoService rolPermisoService)
        {
            _rolPermisoService = rolPermisoService;
        }

        [HttpGet("ListarPorRol/{idRol}")]
        public async Task<IActionResult> ListarPorRol(int idRol)
        {
            try
            {
                var lista = await _rolPermisoService.ListarPermisosPorRol(idRol);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("AsignarPermiso/{idRol}/{idPermiso}")]
        public async Task<IActionResult> AsignarPermiso(int idRol, int idPermiso)
        {
            try
            {
                await _rolPermisoService.AsignarPermiso(idRol, idPermiso);
                return StatusCode(201, new { msj = "Permiso asignado correctamente al rol." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpDelete("RemoverPermiso/{idRol}/{idPermiso}")]
        public async Task<IActionResult> RemoverPermiso(int idRol, int idPermiso)
        {
            try
            {
                await _rolPermisoService.RemoverPermiso(idRol, idPermiso);
                return Ok(new { msj = "Permiso removido correctamente del rol." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}