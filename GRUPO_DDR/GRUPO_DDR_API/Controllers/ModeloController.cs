using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeloController : ControllerBase
    {
        private readonly IModeloService _modeloService;

        public ModeloController(IModeloService modeloService)
        {
            _modeloService = modeloService;
        }

        [HttpGet("ListarModelo")]
        public async Task<IActionResult> ListarModelo()
        {
            try
            {
                var result = await _modeloService.ListarModelo();
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
                var result = await _modeloService.ListarPorNombre(Buscar);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoModelo")]
        public async Task<IActionResult> NuevoModelo([FromBody] ModeloDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _modeloService.NuevoModelo(dto);
                return StatusCode(201, new { msj = "Modelo creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarModelo/{id}")]
        public async Task<IActionResult> EditarModelo(int id, [FromBody] ModeloDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                await _modeloService.EditarModelo(dto);
                return Ok(new { msj = "Modelo actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarModelo/{id}")]
        public async Task<IActionResult> EliminarModelo(int id)
        {
            try
            {
                await _modeloService.EliminarModelo(id);
                return Ok(new { msj = "Modelo eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}