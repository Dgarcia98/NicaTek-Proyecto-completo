using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodegaProductoController : ControllerBase
    {
        private readonly IBodegaProductoService _service;

        public BodegaProductoController(IBodegaProductoService service)
        {
            _service = service;
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            try { return Ok(await _service.ListarBodegaProducto()); }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpGet("Filtrar/{filtro}")]
        public async Task<IActionResult> Filtrar(string filtro)
        {
            try { return Ok(await _service.FiltrarBodegaProducto(filtro)); }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpGet("ListarPorBodega/{idBodega}")]
        public async Task<IActionResult> ListarPorBodega(int idBodega)
        {
            try { return Ok(await _service.ListarPorBodega(idBodega)); }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpPost("Nuevo")]
        public async Task<IActionResult> Nuevo([FromBody] BodegaProductoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { msj = "Modelo inválido." });
                await _service.NuevoBodegaProducto(dto);
                return StatusCode(201, new { msj = "Producto agregado a bodega correctamente" });
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpPut("Editar/{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] BodegaProductoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { msj = "Modelo inválido." });
                if (id != dto.Id_Bodega_Producto) return BadRequest(new { msj = "ID no coincide." });
                await _service.EditarBodegaProducto(dto);
                return Ok(new { msj = "Stock actualizado correctamente" });
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpDelete("Eliminar/{id}/{idModificador}")]
        public async Task<IActionResult> Eliminar(int id, int idModificador)
        {
            try
            {
                await _service.EliminarBodegaProducto(id, idModificador);
                return Ok(new { msj = "Producto eliminado de la bodega correctamente" });
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }
    }
}