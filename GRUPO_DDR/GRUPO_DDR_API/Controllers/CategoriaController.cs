using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpGet("ListarCategoria")]
        public async Task<IActionResult> ListarCategoria()
        {
            try
            {
                var lista = await _service.ListarCategoria();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerCategoriaPorId/{id}")]
        public async Task<IActionResult> ObtenerCategoriaPorId(int id)
        {
            try
            {
                var item = await _service.ObtenerCategoriaPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró la categoría con ID {id}" });
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

        [HttpPost("NuevoCategoria")]
        public async Task<IActionResult> NuevoCategoria([FromBody] CategoriaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoCategoria(dto);
                return StatusCode(201, new { msj = "Categoría creada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarCategoria/{id}")]
        public async Task<IActionResult> EditarCategoria(int id, [FromBody] CategoriaDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Categoria)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.EditarCategoria(dto);
                return Ok(new { msj = "Categoría actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarCategoria/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarCategoria(int id, int idModificador)
        {
            try
            {
                var item = await _service.ObtenerCategoriaPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "La categoría que intenta eliminar no existe." });
                }

                await _service.EliminarCategoria(id, idModificador);
                return Ok(new { msj = "Categoría eliminada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}