using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet("ListarProducto")]
        public async Task<IActionResult> ListarProducto()
        {
            try
            {
                var lista = await _productoService.ListarProducto();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerProductoPorId/{id}")]
        public async Task<IActionResult> ObtenerProductoPorId(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerProductoPorId(id);

                if (producto == null)
                {
                    return NotFound(new { msj = $"No se encontró el producto con ID {id}" });
                }

                return Ok(producto);
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
                if (string.IsNullOrWhiteSpace(Buscar))
                {
                    return BadRequest(new { msj = "El filtro no puede estar vacío" });
                }

                var lista = await _productoService.ListarPorNombre(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoProducto")]
        public async Task<IActionResult> NuevoProducto([FromBody] ProductoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _productoService.NuevoProducto(dto);
                return StatusCode(201, new { msj = "Producto creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarProducto/{id}")]
        public async Task<IActionResult> EditarProducto(int id, [FromBody] ProductoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                await _productoService.EditarProducto(dto);
                return Ok(new { msj = "Producto actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarProducto/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            try
            {
                var producto = await _productoService.ObtenerProductoPorId(id);
                if (producto == null)
                {
                    return NotFound(new { msj = "El producto que intenta eliminar no existe." });
                }

                await _productoService.EliminarProducto(id);
                return Ok(new { msj = "Producto eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}