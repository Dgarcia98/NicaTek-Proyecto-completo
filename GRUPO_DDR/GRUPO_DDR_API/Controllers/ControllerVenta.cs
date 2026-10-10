using GRUPO_DDR_API.Authorization;
using GRUPO_DDR_API.Middleware;
using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GRUPO_DDR_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly IUsuarioActual _usuarioActual;

        public VentaController(IVentaService ventaService, IUsuarioActual usuarioActual)
        {
            _ventaService = ventaService;
            _usuarioActual = usuarioActual;
        }

        [RequierePermiso("POS_SALE")]
        [HttpGet("ListarVentas")]
        public async Task<IActionResult> ListarVentas()
            => Ok(await _ventaService.ListarVentas());

        [RequierePermiso("POS_SALE")]
        [HttpGet("ObtenerVentaPorId/{id}")]
        public async Task<IActionResult> ObtenerVentaPorId(int id)
        {
            var item = await _ventaService.ObtenerVentaPorId(id);
            if (item == null)
                return NotFound(new { exito = false, mensaje = $"No se encontro la venta con ID {id}" });
            return Ok(item);
        }

        [RequierePermiso("POS_SALE")]
        [HttpGet("ListarPorFiltro/{Buscar}")]
        public async Task<IActionResult> ListarPorFiltro(string Buscar)
            => Ok(await _ventaService.ListarVentasFiltro(Buscar));

        /// <summary>
        /// Registro atomico: cabecera, detalles y movimientos de inventario
        /// en una sola transaccion.
        ///
        /// Sustituye al par NuevaVenta + N llamadas a NuevoDetalleVenta que
        /// hacia el frontend, donde un fallo parcial dejaba la venta a medio
        /// guardar con el stock ya descontado.
        ///
        /// El precio, el total y el usuario creador NO se aceptan del
        /// cliente: los determina el servidor.
        /// </summary>
        [RequierePermiso("POS_SALE")]
        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar([FromBody] VentaRegistroDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _ventaService.RegistrarVenta(dto, _usuarioActual.IdUsuarioRequerido);

            if (!resultado.Exito)
                throw new ReglaNegocioException(resultado.Mensaje);

            return StatusCode(201, new
            {
                exito = true,
                mensaje = resultado.Mensaje,
                datos = resultado.Id,
                id_Venta = resultado.Id
            });
        }

        /// <summary>
        /// OBSOLETO. Crea solo la cabecera y deja la venta sin detalles ni
        /// stock movido. Se mantiene mientras quede algun cliente viejo
        /// desplegado. Use Registrar.
        /// </summary>
        [Obsolete("Use POST api/Venta/Registrar, que es transaccional.")]
        [RequierePermiso("POS_SALE")]
        [HttpPost("NuevaVenta")]
        public async Task<IActionResult> NuevaVenta([FromBody] VentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { exito = false, mensaje = "El modelo no es valido." });

            dto.Id_Creador = _usuarioActual.IdUsuarioRequerido;
            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            int nuevoId = await _ventaService.NuevaVenta(dto);
            if (nuevoId > 0)
                return StatusCode(201, new { exito = true, mensaje = "Venta registrada correctamente", datos = nuevoId });

            return BadRequest(new { exito = false, mensaje = "No se pudo generar el ID de la venta." });
        }

        [RequierePermiso("POS_SALE")]
        [HttpPut("EditarVenta/{id}")]
        public async Task<IActionResult> EditarVenta(int id, [FromBody] VentaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { exito = false, mensaje = "El modelo no es valido." });
            if (id != dto.Id_Venta)
                return BadRequest(new { exito = false, mensaje = "El ID de la URL no coincide con el del cuerpo." });

            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            await _ventaService.EditarVenta(dto);
            return Ok(new { exito = true, mensaje = "Venta actualizada correctamente" });
        }

        [RequierePermiso("POS_SALE")]
        [HttpPut("CompletarVenta/{id}")]
        public async Task<IActionResult> CompletarVenta(int id, [FromBody] VentaDTO dto)
        {
            dto.Id_Venta = id;
            dto.Id_Estado = 4; // Completado
            dto.Id_Modificador = _usuarioActual.IdUsuarioRequerido;

            await _ventaService.EditarVenta(dto);
            return Ok(new { exito = true, mensaje = "Venta completada correctamente" });
        }

        [RequierePermiso("INV_DELETE")]
        [HttpDelete("EliminarVenta/{id}")]
        public async Task<IActionResult> EliminarVenta(int id)
        {
            await _ventaService.EliminarVenta(id, _usuarioActual.IdUsuarioRequerido);
            return Ok(new { exito = true, mensaje = "Venta anulada correctamente" });
        }

        /// <summary>
        /// Compatibilidad: el frontend viejo mandaba idModificador en la
        /// URL. Se ignora y se usa el del token. Eliminar tras el despliegue.
        /// </summary>
        [RequierePermiso("INV_DELETE")]
        [HttpDelete("EliminarVenta/{id}/{idModificador}")]
        public Task<IActionResult> EliminarVentaLegacy(int id, int idModificador)
            => EliminarVenta(id);
    }
}
