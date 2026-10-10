using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoMovimientoController : ControllerBase
    {
        private readonly ITipoMovimientoService _service;

        public TipoMovimientoController(ITipoMovimientoService service)
        {
            _service = service;
        }

        [HttpGet("ListarTipoMovimiento")]
        public async Task<IActionResult> ListarTipoMovimiento()
        {
            try
            {
                var lista = await _service.ListarTipoMovimiento();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerTipoMovimientoPorId/{id}")]
        public async Task<IActionResult> ObtenerTipoMovimientoPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerTipoMovimientoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el tipo de movimiento con ID {id}" });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ListarPorNombre/{Buscar}")]
        public async Task<IActionResult> ListarPorNombre(string Buscar)
        {
            try
            {
                var lista = await _service.ListarPorNombre(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoTipoMovimiento")]
        public async Task<IActionResult> NuevoTipoMovimiento([FromBody] TipoMovimientoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoTipoMovimiento(dto);
                return StatusCode(201, new { msj = "Tipo de movimiento creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarTipoMovimiento/{id}")]
        public async Task<IActionResult> EditarTipoMovimiento(int id, [FromBody] TipoMovimientoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Tipo_Movimiento)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.EditarTipoMovimiento(dto);
                return Ok(new { msj = "Tipo de movimiento actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarTipoMovimiento/{id}")]
        public async Task<IActionResult> EliminarTipoMovimiento(int id)
        {
            try
            {
                var item = await _service.ObtenerTipoMovimientoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El tipo de movimiento que intenta eliminar no existe." });
                }

                await _service.EliminarTipoMovimiento(id);
                return Ok(new { msj = "Tipo de movimiento eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}