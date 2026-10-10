using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadoController(IEmpleadoService empleadoService)
        {
            _empleadoService = empleadoService;
        }

        [HttpGet("ListarEmpleado")]
        public async Task<IActionResult> ListarEmpleado()
        {
            try
            {
                var lista = await _empleadoService.ListarEmpleado();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerEmpleadoPorId/{id}")]
        public async Task<IActionResult> ObtenerEmpleadoPorId(int id)
        {
            try
            {
                var item = await _empleadoService.ObtenerEmpleadoPorId(id);

                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el empleado con ID {id}" });
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
                var lista = await _empleadoService.ListarPorFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoEmpleado")]
        public async Task<IActionResult> NuevoEmpleado([FromBody] EmpleadoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _empleadoService.NuevoEmpleado(dto);
                return StatusCode(201, new { msj = "Empleado creado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarEmpleado/{id}")]
        public async Task<IActionResult> EditarEmpleado(int id, [FromBody] EmpleadoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Empleado)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _empleadoService.EditarEmpleado(dto);
                return Ok(new { msj = "Empleado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarEmpleado/{id}")]
        public async Task<IActionResult> EliminarEmpleado(int id)
        {
            try
            {
                var item = await _empleadoService.ObtenerEmpleadoPorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El empleado que intenta eliminar no existe." });
                }

                await _empleadoService.EliminarEmpleado(id);
                return Ok(new { msj = "Empleado eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}