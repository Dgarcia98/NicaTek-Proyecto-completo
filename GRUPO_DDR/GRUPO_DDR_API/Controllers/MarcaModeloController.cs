using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcaModeloController : ControllerBase
    {
        private readonly IMarcaModeloService _service;

        public MarcaModeloController(IMarcaModeloService service)
        {
            _service = service;
        }

        [HttpGet("ListarMarcaModelo")]
        public async Task<IActionResult> ListarMarcaModelo()
        {
            try
            {
                var result = await _service.ListarMarcaModelo();
                return Ok(result);
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
                var result = await _service.ListarPorNombre(Buscar);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoMarcaModelo")]
        public async Task<IActionResult> NuevoMarcaModelo([FromBody] MarcaModeloDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoMarcaModelo(dto);
                return StatusCode(201, new { msj = "Relación Marca-Modelo creada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarMarcaModelo/{id}")]
        public async Task<IActionResult> EditarMarcaModelo(int id, [FromBody] MarcaModeloDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }


                await _service.EditarMarcaModelo(dto);
                return Ok(new { msj = "Relación Marca-Modelo actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarMarcaModelo/{id}")]
        public async Task<IActionResult> EliminarMarcaModelo(int id)
        {
            try
            {
                await _service.EliminarMarcaModelo(id);
                return Ok(new { msj = "Relación Marca-Modelo eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}