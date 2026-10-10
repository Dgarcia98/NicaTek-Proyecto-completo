using System.Data;
using GRUPO_DDR_API.Authorization;
using GRUPO_DDR_INFRASTUCTURE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GRUPO_DDR_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DbConection _conexion;

        public DashboardController(DbConection conexion) => _conexion = conexion;

        /// <summary>
        /// Metricas ya agregadas en SQL.
        ///
        /// Antes el dashboard pedia ListarVentas + ListarCompra +
        /// ListarDetalleVentas COMPLETOS y sumaba todo en el navegador con
        /// filter/reduce. Con 10.000 ventas eso son varios MB por carga y
        /// el calculo bloqueaba el hilo principal. Ahora bajan ~20 filas.
        /// </summary>
        [RequierePermiso("RPT_VIEW")]
        [HttpGet("Resumen")]
        public async Task<IActionResult> Resumen([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("sp_Dashboard_Resumen", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Desde", SqlDbType.DateTime).Value = (object?)desde ?? DBNull.Value;
            cmd.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = (object?)hasta ?? DBNull.Value;

            using var dr = await cmd.ExecuteReaderAsync();

            // El SP devuelve 5 result sets, en este orden.
            var totales = (await LeerAsync(dr)).FirstOrDefault();
            await dr.NextResultAsync();
            var serieMensual = await LeerAsync(dr);
            await dr.NextResultAsync();
            var topProductos = await LeerAsync(dr);
            await dr.NextResultAsync();
            var ultimasVentas = await LeerAsync(dr);
            await dr.NextResultAsync();
            var ultimasCompras = await LeerAsync(dr);

            return Ok(new
            {
                totales,
                serieMensual,
                topProductos,
                ultimasVentas,
                ultimasCompras
            });
        }

        /// <summary>
        /// Vuelca el result set actual a diccionarios, con la primera letra
        /// del nombre de columna en minuscula para que el JSON coincida con
        /// la convencion camelCase que ya consume el frontend.
        /// </summary>
        private static async Task<List<Dictionary<string, object?>>> LeerAsync(SqlDataReader dr)
        {
            var filas = new List<Dictionary<string, object?>>();

            while (await dr.ReadAsync())
            {
                var fila = new Dictionary<string, object?>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    var nombre = dr.GetName(i);
                    var clave = char.ToLowerInvariant(nombre[0]) + nombre.Substring(1);
                    fila[clave] = dr.IsDBNull(i) ? null : dr.GetValue(i);
                }
                filas.Add(fila);
            }

            return filas;
        }
    }
}
