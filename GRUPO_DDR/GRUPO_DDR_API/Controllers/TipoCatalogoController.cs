using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoCatalogoController : ControllerBase
    {
        private readonly ITipoCatalogoService _tipoCatalogoService;

        public TipoCatalogoController(ITipoCatalogoService tipoCatalogoService)
        {
            _tipoCatalogoService = tipoCatalogoService;
        }

        [HttpGet("ListarTipoCatalogo")]
        public async Task<IActionResult> ListarTipoCatalogo()
        {
            try
            {
                var lista = await _tipoCatalogoService.ListarTipoCatalogo();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerTipoCatalogoPorId/{id}")]
        public async Task<IActionResult> ObtenerTipoCatalogoPorId(int id)
        {
            try
            {
                var item = await _tipoCatalogoService.ObtenerTipoCatalogoPorId(id);

                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el Tipo de Catálogo con ID {id}" });
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
                var lista = await _tipoCatalogoService.ListarPorNombre(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoTipoCatalogo")]
        public async Task<IActionResult> NuevoTipoCatalogo([FromBody] TipoCatalogoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _tipoCatalogoService.NuevoTipoCatalogo(dto);
                return StatusCode(201, new { msj = "Tipo de Catálogo creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarTipoCatalogo/{id}")]
        public async Task<IActionResult> EditarTipoCatalogo(int id, [FromBody] TipoCatalogoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Tipo_Catalogo)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _tipoCatalogoService.EditarTipoCatalogo(dto);
                return Ok(new { msj = "Tipo de Catálogo actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarTipoCatalogo/{id}")]
        public async Task<IActionResult> EliminarTipoCatalogo(int id)
        {
            try
            {
                var item = await _tipoCatalogoService.ObtenerTipoCatalogoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El Tipo de Catálogo que intenta eliminar no existe." });
                }

                await _tipoCatalogoService.EliminarTipoCatalogo(id);
                return Ok(new { msj = "Tipo de Catálogo eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}