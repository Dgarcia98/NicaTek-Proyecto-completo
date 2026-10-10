using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using GRUPO_DDR_INFRASTRUCTURE.Data;
using GRUPO_DDR_INFRASTUCTURE.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GRUPO_DDR_INFRASTRUCTURE.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly DbConection _conexion;

        public VentaRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Venta>> ListarVentasAsync()
        {
            var olist = new List<Venta>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Ventas", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Venta
                        {
                            Id_Venta = Convert.ToInt32(dr["Id_Venta"]),
                            Numero_Factura = dr["Numero_Factura"] != DBNull.Value ? dr["Numero_Factura"].ToString() : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Id_Cliente = Convert.ToInt32(dr["Id_Cliente"]),
                            Cliente = dr["Cliente"].ToString(),
                            Id_Empleado = dr["Id_Empleado"] != DBNull.Value ? Convert.ToInt32(dr["Id_Empleado"]) : (int?)null,
                            Empleado = dr["Empleado"] != DBNull.Value ? dr["Empleado"].ToString() : null,
                             Id_Metodo_Pago = dr["Id_Metodo_Pago"] != DBNull.Value ? Convert.ToInt32(dr["Id_Metodo_Pago"]) : (int?)null,
                            Metodo_Pago = dr["Metodo_Pago"] != DBNull.Value ? dr["Metodo_Pago"].ToString() : null,
                            Total = Convert.ToDecimal(dr["Total"]),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Venta>> ListarVentasFiltroAsync(string filtro)
        {
            var olist = new List<Venta>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Ventas", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Venta
                        {
                            Id_Venta = Convert.ToInt32(dr["Id_Venta"]),
                            Numero_Factura = dr["Numero_Factura"] != DBNull.Value ? dr["Numero_Factura"].ToString() : null,
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Id_Cliente = Convert.ToInt32(dr["Id_Cliente"]),
                            Cliente = dr["Cliente"].ToString(),
                            Id_Empleado = dr["Id_Empleado"] != DBNull.Value ? Convert.ToInt32(dr["Id_Empleado"]) : (int?)null,
                            Empleado = dr["Empleado"] != DBNull.Value ? dr["Empleado"].ToString() : null,
                            Id_Metodo_Pago = dr["Id_Metodo_Pago"] != DBNull.Value ? Convert.ToInt32(dr["Id_Metodo_Pago"]) : (int?)null,
                            Metodo_Pago = dr["Metodo_Pago"] != DBNull.Value ? dr["Metodo_Pago"].ToString() : null,
                            Total = Convert.ToDecimal(dr["Total"]),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        /// <summary>
        /// Envia todas las lineas de la venta de un solo golpe mediante un
        /// table-valued parameter, para que el procedimiento las procese
        /// dentro de una unica transaccion.
        /// </summary>
        public async Task<(int Id, string Mensaje)> RegistrarVentaAsync(
            int idCliente,
            int? idMetodoPago,
            DateTime? fecha,
            int idEstado,
            int idUsuario,
            IEnumerable<(int IdProducto, int? IdBodega, int Cantidad)> detalles)
        {
            // Las columnas deben coincidir en nombre y orden con el tipo
            // dbo.TVP_Detalle_Venta definido en 003_registro_transaccional.sql
            var tabla = new DataTable();
            tabla.Columns.Add("Id_Producto", typeof(int));
            tabla.Columns.Add("Id_Bodega", typeof(int));
            tabla.Columns.Add("Cantidad", typeof(int));

            foreach (var d in detalles)
            {
                tabla.Rows.Add(
                    d.IdProducto,
                    d.IdBodega.HasValue ? d.IdBodega.Value : (object)DBNull.Value,
                    d.Cantidad);
            }

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("sp_Venta_Registrar", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Id_Cliente", SqlDbType.Int).Value = idCliente;
            cmd.Parameters.Add("@Id_Metodo_Pago", SqlDbType.Int).Value =
                idMetodoPago.HasValue ? idMetodoPago.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value =
                fecha.HasValue ? fecha.Value : (object)DBNull.Value;
            cmd.Parameters.Add("@Id_Estado", SqlDbType.Int).Value = idEstado;
            cmd.Parameters.Add("@Id_Usuario", SqlDbType.Int).Value = idUsuario;

            var tvp = cmd.Parameters.AddWithValue("@Detalles", tabla);
            tvp.SqlDbType = SqlDbType.Structured;
            tvp.TypeName = "dbo.TVP_Detalle_Venta";

            var oNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var oMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(oNum);
            cmd.Parameters.Add(oMsg);

            await cmd.ExecuteNonQueryAsync();

            int id = oNum.Value != DBNull.Value ? Convert.ToInt32(oNum.Value) : -1;
            string msg = oMsg.Value != DBNull.Value
                ? oMsg.Value.ToString()
                : "Respuesta vacia del procedimiento.";

            return (id, msg);
        }

        public async Task<int> NuevaVentaAsync(Venta venta)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Ventas", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Fecha", venta.Fecha != DateTime.MinValue ? venta.Fecha : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Cliente", venta.Id_Cliente));
                cmd.Parameters.Add(new SqlParameter("@Id_Empleado", venta.Id_Empleado ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Metodo_Pago", venta.Id_Metodo_Pago ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Total", venta.Total));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", venta.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", venta.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", venta.Id_Modificador ?? (object)DBNull.Value));
                var outParamMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outParamMsg);

                var outParamNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(outParamNum);

                await cmd.ExecuteNonQueryAsync();

                int nuevoId = outParamNum.Value != DBNull.Value ? Convert.ToInt32(outParamNum.Value) : 0;
                string mensajeError = outParamMsg.Value != DBNull.Value ? outParamMsg.Value.ToString() : "Error desconocido en BD";

                if (nuevoId <= 0)
                {
                    throw new Exception($"No se pudo registrar la venta. Motivo: {mensajeError}");
                }

                return nuevoId;
            }
        }

        public async Task EditarVentaAsync(Venta venta)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Ventas", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Venta", venta.Id_Venta));
                cmd.Parameters.Add(new SqlParameter("@Fecha", venta.Fecha != DateTime.MinValue ? venta.Fecha : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Cliente", venta.Id_Cliente));
                cmd.Parameters.Add(new SqlParameter("@Id_Empleado", venta.Id_Empleado ?? (object)DBNull.Value));
                 cmd.Parameters.Add(new SqlParameter("@Id_Metodo_Pago", venta.Id_Metodo_Pago ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Total", venta.Total));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", venta.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", venta.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarVentaAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Ventas", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Venta", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}