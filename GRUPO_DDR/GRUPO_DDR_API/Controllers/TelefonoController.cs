using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelefonoController : ControllerBase
    {
        private readonly ITelefonoService _telefonoService;

        public TelefonoController(ITelefonoService telefonoService)
        {
            _telefonoService = telefonoService;
        }

        [HttpGet("ListarTelefono")]
        public async Task<IActionResult> ListarTelefono()
        {
            try
            {
                var lista = await _telefonoService.ListarTelefono();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerTelefonoPorId/{id}")]
        public async Task<IActionResult> ObtenerTelefonoPorId(int id)
        {
            try
            {
                var telefono = await _telefonoService.ObtenerTelefonoPorId(id);

                if (telefono == null)
                {
                    return NotFound(new { msj = $"No se encontró el teléfono con ID {id}" });
                }

                return Ok(telefono);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ListarPorNumero/{Buscar}")]
        public async Task<IActionResult> ListarPorNumero(string Buscar)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Buscar))
                {
                    return BadRequest(new { msj = "El filtro no puede estar vacío" });
                }

                var lista = await _telefonoService.ListarPorNumero(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoTelefono")]
        public async Task<IActionResult> NuevoTelefono([FromBody] TelefonoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _telefonoService.NuevoTelefono(dto);
                return StatusCode(201, new { msj = "Teléfono creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarTelefono/{id}")]
        public async Task<IActionResult> EditarTelefono(int id, [FromBody] TelefonoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                // Aseguramos que el ID del DTO sea el mismo de la ruta
                dto.Id_Telefono = id;

                await _telefonoService.EditarTelefono(dto);
                return Ok(new { msj = "Teléfono actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarTelefono/{id}")]
        public async Task<IActionResult> EliminarTelefono(int id)
        {
            try
            {
                var telefono = await _telefonoService.ObtenerTelefonoPorId(id);
                if (telefono == null)
                {
                    return NotFound(new { msj = "El teléfono que intenta eliminar no existe." });
                }

                await _telefonoService.EliminarTelefono(id);
                return Ok(new { msj = "Teléfono eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}