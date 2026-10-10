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
    public class DetalleVentaRepository : IDetalleVentaRepository
    {
        private readonly DbConection _conexion;

        public DetalleVentaRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<DetalleVenta>> ListarDetalleVentaAsync()
        {
            var olist = new List<DetalleVenta>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Venta", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DetalleVenta
                        {
                            Id_Detalle = Convert.ToInt32(dr["Id_Detalle"]),
                            Id_Venta = Convert.ToInt32(dr["Id_Venta"]),
                            Total_Venta_General = Convert.ToDecimal(dr["Total_Venta_General"]),
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Producto = dr["Producto"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Precio_Unitario_Al_Momento_Venta = Convert.ToDecimal(dr["Precio_Unitario_Al_Momento_Venta"]),
                            Subtotal = Convert.ToDecimal(dr["Subtotal"]),
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

        public async Task<IEnumerable<DetalleVenta>> ListarDetalleVentaFiltroAsync(string filtro)
        {
            var olist = new List<DetalleVenta>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Venta", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DetalleVenta
                        {
                            Id_Detalle = Convert.ToInt32(dr["Id_Detalle"]),
                            Id_Venta = Convert.ToInt32(dr["Id_Venta"]),
                            Total_Venta_General = Convert.ToDecimal(dr["Total_Venta_General"]),
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Producto = dr["Producto"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Precio_Unitario_Al_Momento_Venta = Convert.ToDecimal(dr["Precio_Unitario_Al_Momento_Venta"]),
                            Subtotal = Convert.ToDecimal(dr["Subtotal"]),
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

        public async Task NuevoDetalleVentaAsync(DetalleVenta detalle)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Venta", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Id_Venta", detalle.Id_Venta));
                cmd.Parameters.Add(new SqlParameter("@Id_Producto", detalle.Id_Producto));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", detalle.Cantidad));
                cmd.Parameters.Add(new SqlParameter("@Precio_Unitario_Al_Momento_Venta", detalle.Precio_Unitario_Al_Momento_Venta));
                cmd.Parameters.Add(new SqlParameter("@Subtotal", detalle.Subtotal > 0 ? detalle.Subtotal : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", detalle.Id_Estado > 0 ? detalle.Id_Estado : 1));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", detalle.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", detalle.Id_Modificador > 0 ? detalle.Id_Modificador : detalle.Id_Creador));

                // Declaramos explícitamente los parámetros de salida para poder leerlos después
                var outParamMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
                var outParamNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outParamMsg);
                cmd.Parameters.Add(outParamNum);

                await cmd.ExecuteNonQueryAsync();

                // Leemos lo que nos devolvió el Stored Procedure
                int statusNum = outParamNum.Value != DBNull.Value ? Convert.ToInt32(outParamNum.Value) : 0;
                string mensajeError = outParamMsg.Value != DBNull.Value ? outParamMsg.Value.ToString() : "Error desconocido al insertar el detalle";

                // Si el número es 0 o -1, significa que el SP detectó un error (validación o Catch)
                if (statusNum <= 0)
                {
                    throw new Exception($"Error en Detalle (Producto ID {detalle.Id_Producto}): {mensajeError}");
                }
            }
        }

        public async Task EditarDetalleVentaAsync(DetalleVenta detalle)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Venta", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Detalle", detalle.Id_Detalle));
                cmd.Parameters.Add(new SqlParameter("@Id_Producto", detalle.Id_Producto != 0 ? detalle.Id_Producto : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", detalle.Cantidad != 0 ? detalle.Cantidad : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Precio_Unitario_Al_Momento_Venta", detalle.Precio_Unitario_Al_Momento_Venta != 0 ? detalle.Precio_Unitario_Al_Momento_Venta : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Subtotal", detalle.Subtotal > 0 ? detalle.Subtotal : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", detalle.Id_Estado > 0 ? detalle.Id_Estado : DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", detalle.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarDetalleVentaAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Venta", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Detalle", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}