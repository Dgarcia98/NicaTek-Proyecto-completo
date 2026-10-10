using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionController : ControllerBase
    {
        private readonly IDireccionService _service;

        public DireccionController(IDireccionService service)
        {
            _service = service;
        }

        [HttpGet("ListarDirecciones")]
        public async Task<IActionResult> ListarDirecciones()
        {
            try
            {
                var lista = await _service.ListarDirecciones();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerDireccionPorId/{id}")]
        public async Task<IActionResult> ObtenerDireccionPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró la dirección con ID {id}" });
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("FiltrarDirecciones/{filtro}")]
        public async Task<IActionResult> FiltrarDirecciones(string filtro)
        {
            try
            {
                var lista = await _service.FiltrarDirecciones(filtro);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevaDireccion")]
        public async Task<IActionResult> NuevaDireccion([FromBody] DireccionDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.InsertarDireccion(dto);
                return StatusCode(201, new { msj = "Dirección registrada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarDireccion/{id}")]
        public async Task<IActionResult> EditarDireccion(int id, [FromBody] DireccionDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Direccion)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.ActualizarDireccion(dto);
                return Ok(new { msj = "Dirección actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarDireccion/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarDireccion(int id, int idModificador)
        {
            try
            {
                // Primero verificamos si existe, tal como en tu CatalogoController
                var item = await _service.ObtenerPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "La dirección que intenta eliminar no existe." });
                }

                await _service.EliminarDireccion(id, idModificador);
                return Ok(new { msj = "Dirección eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}