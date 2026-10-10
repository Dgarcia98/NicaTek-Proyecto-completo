using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoController : ControllerBase
    {
        private readonly IEstadoService _estadoService;

        public EstadoController(IEstadoService estadoService)
        {
            _estadoService = estadoService;
        }

        [HttpGet("ListarEstado")]
        public async Task<IActionResult> ListarEstado()
        {
            try
            {
                var lista = await _estadoService.ListarEstado();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerEstadoPorId/{id}")]
        public async Task<IActionResult> ObtenerEstadoPorId(int id)
        {
            try
            {
                var item = await _estadoService.ObtenerEstadoPorId(id);

                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el estado con ID {id}" });
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
                var lista = await _estadoService.ListarPorNombre(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoEstado")]
        public async Task<IActionResult> NuevoEstado([FromBody] EstadoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _estadoService.NuevoEstado(dto);
                return StatusCode(201, new { msj = "Estado creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarEstado/{id}")]
        public async Task<IActionResult> EditarEstado(int id, [FromBody] EstadoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Estado)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _estadoService.EditarEstado(dto);
                return Ok(new { msj = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarEstado/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarEstado(int id, int idModificador)
        {
            try
            {
                var item = await _estadoService.ObtenerEstadoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El estado que intenta eliminar no existe." });
                }

                await _estadoService.EliminarEstado(id, idModificador);
                return Ok(new { msj = "Estado eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}