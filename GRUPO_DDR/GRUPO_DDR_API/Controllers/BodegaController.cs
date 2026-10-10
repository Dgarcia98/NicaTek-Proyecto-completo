using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegaController : ControllerBase
    {
        private readonly IBodegaService _bodegaService;

        public BodegaController(IBodegaService bodegaService)
        {
            _bodegaService = bodegaService;
        }

        [HttpGet("ListarBodega")]
        public async Task<IActionResult> ListarBodega()
        {
            try { return Ok(await _bodegaService.ListarBodega()); }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpGet("ObtenerBodegaPorId/{id}")]
        public async Task<IActionResult> ObtenerBodegaPorId(int id)
        {
            try
            {
                var item = await _bodegaService.ObtenerBodegaPorId(id);
                if (item == null) return NotFound(new { msj = $"No se encontró la bodega con ID {id}" });
                return Ok(item);
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpGet("ListarPorNombre/{buscar}")]
        public async Task<IActionResult> ListarPorNombre(string buscar)
        {
            try { return Ok(await _bodegaService.ListarPorNombre(buscar)); }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpPost("NuevoBodega")]
        public async Task<IActionResult> NuevoBodega([FromBody] BodegaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { msj = "Modelo inválido." });
                await _bodegaService.NuevoBodega(dto);
                return StatusCode(201, new { msj = "Bodega creada correctamente" });
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpPut("EditarBodega/{id}")]
        public async Task<IActionResult> EditarBodega(int id, [FromBody] BodegaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { msj = "Modelo inválido." });
                if (id != dto.Id_Bodega) return BadRequest(new { msj = "ID no coincide." });
                await _bodegaService.EditarBodega(dto);
                return Ok(new { msj = "Bodega actualizada correctamente" });
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpDelete("EliminarBodega/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarBodega(int id, int idModificador)
        {
            try
            {
                var item = await _bodegaService.ObtenerBodegaPorId(id);
                if (item == null) return NotFound(new { msj = "La bodega no existe." });
                await _bodegaService.EliminarBodega(id, idModificador);
                return Ok(new { msj = "Bodega desactivada correctamente" });
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }
    }
}