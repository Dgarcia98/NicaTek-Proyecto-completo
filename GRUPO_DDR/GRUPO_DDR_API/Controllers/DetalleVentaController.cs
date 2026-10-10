using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleVentaController : ControllerBase
    {
        private readonly IDetalleVentaService _detalleVentaService;

        public DetalleVentaController(IDetalleVentaService detalleVentaService)
        {
            _detalleVentaService = detalleVentaService;
        }

        [HttpGet("ListarDetalleVentas")]
        public async Task<IActionResult> ListarDetalleVentas()
        {
            try
            {
                var lista = await _detalleVentaService.ListarDetalleVenta();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpGet("ObtenerDetalleVentaPorId/{id}")]
        public async Task<IActionResult> ObtenerDetalleVentaPorId(int id)
        {
            try
            {
                var item = await _detalleVentaService.ObtenerDetalleVentaPorId(id);

                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el detalle de venta con ID {id}" });
                }

                return Ok(item);
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
                var lista = await _detalleVentaService.ListarDetalleVentaFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("NuevoDetalleVenta")]
        public async Task<IActionResult> NuevoDetalleVenta([FromBody] DetalleVentaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { exito = false, msj = "El modelo no es válido, verifique los datos." });
                }

                await _detalleVentaService.NuevoDetalleVenta(dto);
                // IMPORTANTE: Asegúrate de incluir exito = true
                return StatusCode(201, new { exito = true, msj = "Detalle de venta registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { exito = false, msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPut("EditarDetalleVenta/{id}")]
        public async Task<IActionResult> EditarDetalleVenta(int id, [FromBody] DetalleVentaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Detalle)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _detalleVentaService.EditarDetalleVenta(dto);
                return Ok(new { msj = "Detalle de venta actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpDelete("EliminarDetalleVenta/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarDetalleVenta(int id, int idModificador)
        {
            try
            {
                await _detalleVentaService.EliminarDetalleVenta(id, idModificador);
                return Ok(new { msj = "Detalle de venta anulado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno del servidor", error = ex.Message });
            }
        }
    }
}