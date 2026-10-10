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
    public class MovimientoInventarioRepository : IMovimientoInventarioRepository
    {
        private readonly DbConection _conexion;

        public MovimientoInventarioRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<MovimientoInventario>> ListarMovimientosAsync()
        {
            var olist = new List<MovimientoInventario>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Movimientos_Inventario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new MovimientoInventario
                        {
                            Id_Movimiento = Convert.ToInt32(dr["Id_Movimiento"]),
                            Fecha_Movimiento = Convert.ToDateTime(dr["Fecha_Movimiento"]),
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Producto = dr["Producto"].ToString(),
                            Id_Tipo_Movimiento = Convert.ToInt32(dr["Id_Tipo_Movimiento"]),
                            Tipo_Movimiento = dr["Tipo_Movimiento"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Stock_Anterior = Convert.ToInt32(dr["Stock_Anterior"]),
                            Stock_Actual = Convert.ToInt32(dr["Stock_Actual"]),
                            Id_Bodega = Convert.ToInt32(dr["Id_Bodega"]),
                            Costo_Unitario = Convert.ToDecimal(dr["Costo_Unitario"]),
                            Referencia = dr["Referencia"].ToString(),
                            Observaciones = dr["Observaciones"].ToString(),
                            Id_Usuario_Responsable = Convert.ToInt32(dr["Id_Usuario_Responsable"]),
                            Responsable = dr["Responsable"].ToString()
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<MovimientoInventario>> ListarMovimientosFiltroAsync(string filtro)
        {
            var olist = new List<MovimientoInventario>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Movimientos_Inventario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new MovimientoInventario
                        {
                            Id_Movimiento = Convert.ToInt32(dr["Id_Movimiento"]),
                            Fecha_Movimiento = Convert.ToDateTime(dr["Fecha_Movimiento"]),
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Producto = dr["Producto"].ToString(),
                            Id_Tipo_Movimiento = Convert.ToInt32(dr["Id_Tipo_Movimiento"]),
                            Tipo_Movimiento = dr["Tipo_Movimiento"].ToString(),
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Stock_Anterior = Convert.ToInt32(dr["Stock_Anterior"]),
                            Stock_Actual = Convert.ToInt32(dr["Stock_Actual"]),
                            Id_Bodega = Convert.ToInt32(dr["Id_Bodega"]),
                            Costo_Unitario = Convert.ToDecimal(dr["Costo_Unitario"]),
                            Referencia = dr["Referencia"].ToString(),
                            Observaciones = dr["Observaciones"].ToString(),
                            Id_Usuario_Responsable = Convert.ToInt32(dr["Id_Usuario_Responsable"]),
                            Responsable = dr["Responsable"].ToString()
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoMovimientoAsync(MovimientoInventario movimiento)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Movimientos_Inventario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Id_Producto", movimiento.Id_Producto));
                cmd.Parameters.Add(new SqlParameter("@Id_Bodega", movimiento.Id_Bodega));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Movimiento", movimiento.Id_Tipo_Movimiento));
                cmd.Parameters.Add(new SqlParameter("@Cantidad", movimiento.Cantidad));
                cmd.Parameters.Add(new SqlParameter("@Stock_Anterior", movimiento.Stock_Anterior));
                cmd.Parameters.Add(new SqlParameter("@Stock_Actual", movimiento.Stock_Actual));
                cmd.Parameters.Add(new SqlParameter("@Costo_Unitario", movimiento.Costo_Unitario));
                cmd.Parameters.Add(new SqlParameter("@Referencia", string.IsNullOrEmpty(movimiento.Referencia) ? DBNull.Value : movimiento.Referencia));
                cmd.Parameters.Add(new SqlParameter("@Observaciones", string.IsNullOrEmpty(movimiento.Observaciones) ? DBNull.Value : movimiento.Observaciones));
                cmd.Parameters.Add(new SqlParameter("@Id_Usuario_Responsable", movimiento.Id_Usuario_Responsable));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}