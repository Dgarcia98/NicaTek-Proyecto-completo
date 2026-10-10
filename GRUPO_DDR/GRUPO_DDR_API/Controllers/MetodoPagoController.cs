using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MetodoPagoController : ControllerBase
    {
        private readonly IMetodoPagoService _service;

        public MetodoPagoController(IMetodoPagoService service)
        {
            _service = service;
        }

        [HttpGet("ListarMetodoPago")]
        public async Task<IActionResult> ListarMetodoPago()
        {
            try
            {
                var lista = await _service.ListarMetodoPago();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerMetodoPagoPorId/{id}")]
        public async Task<IActionResult> ObtenerMetodoPagoPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerMetodoPagoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el método de pago con ID {id}" });
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

        [HttpPost("NuevoMetodoPago")]
        public async Task<IActionResult> NuevoMetodoPago([FromBody] MetodoPagoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoMetodoPago(dto);
                return StatusCode(201, new { msj = "Método de pago creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarMetodoPago/{id}")]
        public async Task<IActionResult> EditarMetodoPago(int id, [FromBody] MetodoPagoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                await _service.EditarMetodoPago(dto);
                return Ok(new { msj = "Método de pago actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarMetodoPago/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarMetodoPago(int id, int idModificador)
        {
            try
            {
                var item = await _service.ObtenerMetodoPagoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El método de pago que intenta eliminar no existe." });
                }

                await _service.EliminarMetodoPago(id, idModificador);
                return Ok(new { msj = "Método de pago eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}