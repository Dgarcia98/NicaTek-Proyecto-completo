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
    public class DetalleCompraRepository : IDetalleCompraRepository
    {
        private readonly DbConection _conexion;

        public DetalleCompraRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<DetalleCompra>> ListarDetalleCompraAsync()
        {
            var olist = new List<DetalleCompra>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DetalleCompra
                        {
                            Id_Detalle_Compra = Convert.ToInt32(dr["Id_Detalle_Compra"]),
                            Id_Compra = Convert.ToInt32(dr["Id_Compra"]),
                            Total_Compra = Convert.ToDecimal(dr["Total_Compra"]),
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Producto = dr["Producto"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Precio_Unitario_Compra = Convert.ToDecimal(dr["Precio_Unitario_Compra"]),
                            Subtotal_Compra = Convert.ToDecimal(dr["Subtotal_Compra"]),
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

        public async Task<IEnumerable<DetalleCompra>> FiltrarDetalleCompraAsync(string filtro)
        {
            var olist = new List<DetalleCompra>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DetalleCompra
                        {
                            Id_Detalle_Compra = Convert.ToInt32(dr["Id_Detalle_Compra"]),
                            Id_Compra = Convert.ToInt32(dr["Id_Compra"]),
                            Total_Compra = Convert.ToDecimal(dr["Total_Compra"]),
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Producto = dr["Producto"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Precio_Unitario_Compra = Convert.ToDecimal(dr["Precio_Unitario_Compra"]),
                            Subtotal_Compra = Convert.ToDecimal(dr["Subtotal_Compra"]),
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

        public async Task NuevoDetalleCompraAsync(DetalleCompra entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Id_Compra", entidad.Id_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Bodega", entidad.Id_Bodega));  // ← NUEVO
                cmd.Parameters.Add(new SqlParameter("@Id_Producto", entidad.Id_Producto));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", entidad.Cantidad));
                cmd.Parameters.Add(new SqlParameter("@Precio_Unitario_Compra", entidad.Precio_Unitario_Compra));
                cmd.Parameters.Add(new SqlParameter("@Subtotal_Compra", entidad.Subtotal_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarDetalleCompraAsync(DetalleCompra entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Detalle_Compra", entidad.Id_Detalle_Compra));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", entidad.Cantidad));
                cmd.Parameters.Add(new SqlParameter("@Precio_Unitario_Compra", entidad.Precio_Unitario_Compra));
                cmd.Parameters.Add(new SqlParameter("@Subtotal_Compra", entidad.Subtotal_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarDetalleCompraAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Detalle_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Detalle_Compra", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}