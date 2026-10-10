using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisoController : ControllerBase
    {
        private readonly IPermisoService _permisoService;

        public PermisoController(IPermisoService permisoService)
        {
            _permisoService = permisoService;
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            try { return Ok(await _permisoService.ListarPermisos()); }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpGet("ObtenerPorId/{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var item = await _permisoService.ObtenerPermisoPorId(id);
            return item != null ? Ok(item) : NotFound();
        }

        [HttpGet("Filtrar/{busqueda}")]
        public async Task<IActionResult> Filtrar(string busqueda)
        {
            return Ok(await _permisoService.FiltrarPermisos(busqueda));
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] PermisoDTO dto)
        {
            try
            {
                await _permisoService.NuevoPermiso(dto);
                return Ok(new { msj = "Permiso creado con éxito" });
            }
            catch (Exception ex) { return BadRequest(new { msj = ex.Message }); }
        }

        [HttpPut("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] PermisoDTO dto)
        {
            try
            {
                dto.Id_Permiso = id;
                await _permisoService.EditarPermiso(dto);
                return Ok(new { msj = "Permiso actualizado con éxito" });
            }
            catch (Exception ex) { return BadRequest(new { msj = ex.Message }); }
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _permisoService.EliminarPermiso(id);
                return Ok(new { msj = "Permiso inactivado con éxito" });
            }
            catch (Exception ex) { return BadRequest(new { msj = ex.Message }); }
        }
    }
}