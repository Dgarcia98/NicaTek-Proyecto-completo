using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _service;

        public CatalogoController(ICatalogoService service)
        {
            _service = service;
        }

        [HttpGet("ListarCatalogo")]
        public async Task<IActionResult> ListarCatalogo()
        {
            try
            {
                var lista = await _service.ListarCatalogo();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerCatalogoPorId/{id}")]
        public async Task<IActionResult> ObtenerCatalogoPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerCatalogoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el catálogo con ID {id}" });
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

        [HttpPost("NuevoCatalogo")]
        public async Task<IActionResult> NuevoCatalogo([FromBody] CatalogoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoCatalogo(dto);
                return StatusCode(201, new { msj = "Catálogo creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarCatalogo/{id}")]
        public async Task<IActionResult> EditarCatalogo(int id, [FromBody] CatalogoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Catalogo)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.EditarCatalogo(dto);
                return Ok(new { msj = "Catálogo actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarCatalogo/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarCatalogo(int id, int idModificador)
        {
            try
            {
                var item = await _service.ObtenerCatalogoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El catálogo que intenta eliminar no existe." });
                }

                await _service.EliminarCatalogo(id, idModificador);
                return Ok(new { msj = "Catálogo eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}