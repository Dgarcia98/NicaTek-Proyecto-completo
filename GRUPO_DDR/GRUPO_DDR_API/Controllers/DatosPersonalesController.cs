using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosPersonalesController : ControllerBase
    {
        private readonly IDatosPersonalesService _service;

        public DatosPersonalesController(IDatosPersonalesService service)
        {
            _service = service;
        }

        [HttpGet("ListarDatosPersonales")]
        public async Task<IActionResult> ListarDatosPersonales()
        {
            try
            {
                var lista = await _service.ListarDatosPersonales();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerDatosPersonalesPorId/{id}")]
        public async Task<IActionResult> ObtenerDatosPersonalesPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerDatosPersonalesPorId(id);
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

        [HttpPost("NuevoDatosPersonales")]
        public async Task<IActionResult> NuevoDatosPersonales([FromBody] DatosPersonalesDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoDatosPersonales(dto);
                return StatusCode(201, new { msj = "Registro creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarDatosPersonales/{id}")]
        public async Task<IActionResult> EditarDatosPersonales(int id, [FromBody] DatosPersonalesDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Persona)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.EditarDatosPersonales(dto);
                return Ok(new { msj = "Registro actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarDatosPersonales/{id}")]
        public async Task<IActionResult> EliminarDatosPersonales(int id)
        {
            try
            {
                var item = await _service.ObtenerDatosPersonalesPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El registro que intenta eliminar no existe." });
                }

                await _service.EliminarDatosPersonales(id);
                return Ok(new { msj = "Registro eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}