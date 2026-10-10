using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet("ListarRoles")]
        public async Task<IActionResult> ListarRoles()
        {
            try
            {
                var lista = await _rolService.ListarRoles();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("ObtenerRolPorId/{id}")]
        public async Task<IActionResult> ObtenerRolPorId(int id)
        {
            try
            {
                var item = await _rolService.ObtenerRolPorId(id);

                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el rol con ID {id}" });
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("ListarPorFiltro/{Buscar}")]
        public async Task<IActionResult> ListarPorFiltro(string Buscar)
        {
            try
            {
                var lista = await _rolService.ListarRolesFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("NuevoRol")]
        public async Task<IActionResult> NuevoRol([FromBody] RolDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errores = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { campo = x.Key, error = x.Value.Errors.Select(e => e.ErrorMessage) });
                    return BadRequest(new { msj = "El modelo no es válido", errores });
                }

                await _rolService.NuevoRol(dto);
                return StatusCode(201, new { msj = "Rol registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPut("EditarRol/{id}")]
        public async Task<IActionResult> EditarRol(int id, [FromBody] RolDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Rol)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _rolService.EditarRol(dto);
                return Ok(new { msj = "Rol actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpDelete("EliminarRol/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarRol(int id, int idModificador)
        {
            try
            {
                await _rolService.EliminarRol(id, idModificador);
                return Ok(new { msj = "Rol inactivado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}