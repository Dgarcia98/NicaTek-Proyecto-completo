using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClienteController(IClienteService service)
        {
            _service = service;
        }

        [HttpGet("ListarClientes")]
        public async Task<IActionResult> ListarClientes()
        {
            try
            {
                var lista = await _service.ListarClientes();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("ObtenerClientePorId/{id}")]
        public async Task<IActionResult> ObtenerClientePorId(int id)
        {
            try
            {
                var item = await _service.ObtenerClientePorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = $"No se encontró el cliente con ID {id}" });
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
                var lista = await _service.ListarPorFiltro(Buscar);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("NuevoCliente")]
        public async Task<IActionResult> NuevoCliente([FromBody] ClienteDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido, verifique los datos." });
                }

                await _service.NuevoCliente(dto);
                return StatusCode(201, new { msj = "Cliente registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("EditarCliente/{id}")]
        public async Task<IActionResult> EditarCliente(int id, [FromBody] ClienteDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { msj = "El modelo no es válido." });
                }

                if (id != dto.Id_Cliente)
                {
                    return BadRequest(new { msj = "El ID de la URL no coincide con el ID del cuerpo de la petición." });
                }

                await _service.EditarCliente(dto);
                return Ok(new { msj = "Cliente actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("EliminarCliente/{id}/{idModificador}")]
        public async Task<IActionResult> EliminarCliente(int id, int idModificador)
        {
            try
            {
                var item = await _service.ObtenerClientePorId(id);
                if (item == null)
                {
                    return NotFound(new { msj = "El cliente que intenta eliminar no existe." });
                }

                await _service.EliminarCliente(id, idModificador);
                return Ok(new { msj = "Cliente eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msj = "Internal server error", error = ex.Message });
            }
        }
    }
}