using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly IMovimientoInventarioService _movimientoService;

        public MovimientoInventarioController(IMovimientoInventarioService movimientoService)
        {
            _movimientoService = movimientoService;
        }

        [HttpGet("ListarMovimientos")]
        public async Task<IActionResult> ListarMovimientos()
        {
            try
            {
                var lista = await _movimientoService.ListarMovimientos();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("ListarPorFiltro/{Buscar}")]
        public async Task<IActionResult> ListarPorFiltro(string Buscar)
        {
            try
            {
                var lista = await _movimientoService.ListarMovimientosFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("NuevoMovimiento")]
        public async Task<IActionResult> NuevoMovimiento([FromBody] MovimientoInventarioDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _movimientoService.NuevoMovimiento(dto);
                return StatusCode(201, new { msj = "Movimiento de inventario registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}