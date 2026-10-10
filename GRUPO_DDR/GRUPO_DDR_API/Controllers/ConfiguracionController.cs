using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IConfiguracionService _service;

        public ConfiguracionController(IConfiguracionService service)
        {
            _service = service;
        }

        [HttpGet("Obtener")]
        public async Task<IActionResult> Obtener()
        {
            try
            {
                var resultado = await _service.ObtenerConfiguracion();
                return Ok(resultado);
            }
            catch (Exception ex) { return StatusCode(500, new { msj = ex.Message }); }
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] ConfiguracionEmpresaDTO dto)
        {
            try
            {
                await _service.NuevaConfiguracion(dto);
                return Ok(new { msj = "Configuración inicial guardada con éxito" });
            }
            catch (Exception ex) { return BadRequest(new { msj = ex.Message }); }
        }

        [HttpPut("Actualizar/{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ConfiguracionEmpresaDTO dto)
        {
            try
            {
                dto.Id_Configuracion = id;
                await _service.EditarConfiguracion(dto);
                return Ok(new { msj = "Datos de la empresa actualizados con éxito" });
            }
            catch (Exception ex) { return BadRequest(new { msj = ex.Message }); }
        }
    }
}