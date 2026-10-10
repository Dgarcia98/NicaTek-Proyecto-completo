using GRUPO_DDR_API.Authorization;
using GRUPO_DDR_API.Middleware;
using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;
        private readonly IUsuarioActual _usuarioActual;

        public CompraController(ICompraService compraService, IUsuarioActual usuarioActual)
        {
            _compraService = compraService;
            _usuarioActual = usuarioActual;
        }

        [RequierePermiso("INV_READ")]
        [HttpGet("ListarCompra")]
        public async Task<IActionResult> ListarCompra()
            => Ok(await _compraService.ListarCompra());

        [RequierePermiso("INV_READ")]
        [HttpGet("ObtenerCompraPorId/{id}")]
        public async Task<IActionResult> ObtenerCompraPorId(int id)
        {
            var item = await _compraService.ObtenerCompraPorId(id);
            if (item == null)
                return NotFound(new { exito = false, mensaje = $"No se encontro la compra con ID {id}" });
            return Ok(item);
        }

        [RequierePermiso("INV_READ")]
        [HttpGet("ListarPorFiltro/{Buscar}")]
        public async Task<IActionResult> ListarPorFiltro(string Buscar)
            => Ok(await _compraService.ListarPorFiltro(Buscar));

        /// <summary>
        /// Registro atomico: cabecera, detalles y entradas de inventario en
        /// una sola transaccion. El total y el usuario creador los pone el
        /// servidor. Ver VentaController.Registrar para el contexto.
        /// </summary>
        [RequierePermiso("INV_CREATE")]
        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar([FromBody] CompraRegistroDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _compraService.RegistrarCompra(dto, _usuarioActual.IdUsuarioRequerido);

            if (!resultado.Exito)
                throw new ReglaNegocioException(resultado.Mensaje);

            return StatusCode(201, new
            {
                exito = true,
                mensaje = resultado.Mensaje,
                datos = resultado.Id,
                id_Compra = resultado.Id
            });
        }

        /// <summary>OBSOLETO: crea solo la cabecera. Use Registrar.</summary>
        [Obsolete("Use POST api/Compra/Registrar, que es transaccional.")]
        [RequierePermiso("INV_CREATE")]
        [HttpPost("NuevoCompra")]
        public async Task<IActionResult> NuevoCompra([FromBody] CompraDTO dto)
        {
            dto.Id_Creador = _usuarioActual.IdUsuarioRequerido;
            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            int idGenerado = await _compraService.NuevoCompra(dto);
            return StatusCode(201, new { exito = true, mensaje = "Compra registrada correctamente", id_Compra = idGenerado });
        }

        [RequierePermiso("INV_UPDATE")]
        [HttpPut("CompletarCompra/{id}")]
        public async Task<IActionResult> CompletarCompra(int id, [FromBody] CompraDTO dto)
        {
            dto.Id_Compra = id;
            dto.Id_Estado = 4; // Completado
            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            await _compraService.EditarCompra(dto);
            return Ok(new { exito = true, mensaje = "Compra completada correctamente" });
        }

        [RequierePermiso("INV_UPDATE")]
        [HttpPut("EditarCompra/{id}")]
        public async Task<IActionResult> EditarCompra(int id, [FromBody] CompraDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { exito = false, mensaje = "El modelo no es valido." });
            if (id != dto.Id_Compra)
                return BadRequest(new { exito = false, mensaje = "El ID de la URL no coincide con el del cuerpo." });

            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            await _compraService.EditarCompra(dto);
            return Ok(new { exito = true, mensaje = "Compra actualizada correctamente" });
        }

        [RequierePermiso("INV_DELETE")]
        [HttpDelete("EliminarCompra/{id}")]
        public async Task<IActionResult> EliminarCompra(int id)
        {
            await _compraService.EliminarCompra(id, _usuarioActual.IdUsuarioRequerido);
            return Ok(new { exito = true, mensaje = "Compra anulada correctamente" });
        }

        /// <summary>Compatibilidad con el frontend anterior. Eliminar tras el despliegue.</summary>
        [RequierePermiso("INV_DELETE")]
        [HttpDelete("EliminarCompra/{id}/{idModificador}")]
        public Task<IActionResult> EliminarCompraLegacy(int id, int idModificador)
            => EliminarCompra(id);
    }
}
