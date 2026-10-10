using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorreoController : ControllerBase
    {
        private readonly ICorreoService _service;

        public CorreoController(ICorreoService service)
        {
            _service = service;
        }

        [HttpGet("ListarCorreo")]
        public async Task<IActionResult> ListarCorreo()
        {
            try
            {
                var lista = await _service.ListarCorreo();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerCorreoPorId/{id}")]
        public async Task<IActionResult> ObtenerCorreoPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerCorreoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el registro con ID {id}" });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ListarPorFiltro/{Buscar}")]
        public async Task<IActionResult> ListarPorFiltro(string Buscar)
        {
            try
            {
                var lista = await _service.ListarPorFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoCorreo")]
        public async Task<IActionResult> NuevoCorreo([FromBody] CorreoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoCorreo(dto);
                return StatusCode(201, new { msj = "Registro creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarCorreo/{id}")]
        public async Task<IActionResult> EditarCorreo(int id, [FromBody] CorreoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Correo)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.EditarCorreo(dto);
                return Ok(new { msj = "Registro actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarCorreo/{id}")]
        public async Task<IActionResult> EliminarCorreo(int id, [FromQuery] int idModificador)
        {
            try
            {
                var item = await _service.ObtenerCorreoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El registro que intenta eliminar no existe." });
                }

                // Se pasa el idModificador recibido en la Query String
                await _service.EliminarCorreo(id, idModificador);
                return Ok(new { msj = "Registro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}