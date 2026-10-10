using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCompraController : ControllerBase
    {
        private readonly IDetalleCompraService _detalleCompraService;

        public DetalleCompraController(IDetalleCompraService detalleCompraService)
        {
            _detalleCompraService = detalleCompraService;
        }

        [HttpGet("ListarDetalleCompra")]
        public async Task<IActionResult> ListarDetalleCompra()
        {
            try
            {
                var lista = await _detalleCompraService.ListarDetalleCompra();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerDetalleCompraPorId/{id}")]
        public async Task<IActionResult> ObtenerDetalleCompraPorId(int id)
        {
            try
            {
                var item = await _detalleCompraService.ObtenerDetalleCompraPorId(id);

                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el detalle de compra con ID {id}" });
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
                var lista = await _detalleCompraService.ListarPorFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoDetalleCompra")]
        public async Task<IActionResult> NuevoDetalleCompra([FromBody] DetalleCompraDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _detalleCompraService.NuevoDetalleCompra(dto);
                return StatusCode(201, new { msj = "Detalle de compra registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarDetalleCompra/{id}")]
        public async Task<IActionResult> EditarDetalleCompra(int id, [FromBody] DetalleCompraDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Detalle_Compra)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _detalleCompraService.EditarDetalleCompra(dto);
                return Ok(new { msj = "Detalle de compra actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarDetalleCompra/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarDetalleCompra(int id, int idModificador)
        {
            try
            {
                await _detalleCompraService.EliminarDetalleCompra(id, idModificador);
                return Ok(new { msj = "Detalle de compra eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}