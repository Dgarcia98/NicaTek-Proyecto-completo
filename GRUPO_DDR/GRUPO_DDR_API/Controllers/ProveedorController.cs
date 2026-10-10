using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedoresController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            try
            {
                var lista = await _proveedorService.ListarProveedor();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno", error = ex.Message });
            }
        }

        [HttpGet("obtener/{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var proveedor = await _proveedorService.ObtenerProveedorPorId(id);
                if (proveedor == null) return NotFound(new { msj = $"Proveedor {id} no encontrado" });
                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno", error = ex.Message });
            }
        }

        [HttpGet("buscar/{nombre}")]
        public async Task<IActionResult> Buscar(string nombre)
        {
            try
            {
                var lista = await _proveedorService.ListarPorNombre(nombre);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno", error = ex.Message });
            }
        }

        [HttpPost("insertar")]
        public async Task<IActionResult> Crear([FromBody] ProveedorDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _proveedorService.NuevoProveedor(dto);
                return StatusCode(201, new { msj = "Proveedor registrado con éxito" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno", error = ex.Message });
            }
        }


        [HttpPut("editar/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ProveedorDTO dto)
        {
            try
            {
                dto.Id_Proveedor = id; // Aseguramos que el ID coincida con la URL
                await _proveedorService.EditarProveedor(dto);
                return Ok(new { msj = "Proveedor actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno", error = ex.Message });
            }
        }
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _proveedorService.EliminarProveedor(id);
                return Ok(new { msj = "Proveedor eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Error interno", error = ex.Message });
            }
        }
    }
}