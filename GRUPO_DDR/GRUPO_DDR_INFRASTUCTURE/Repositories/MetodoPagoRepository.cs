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
    public class MetodoPagoRepository : IMetodoPagoRepository
    {
        private readonly DbConection _conexion;

        public MetodoPagoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<MetodoPago>> ListarMetodosPagoAsync()
        {
            var olist = new List<MetodoPago>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsMetodosPago", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new MetodoPago
                        {
                            Id_Metodo_Pago = Convert.ToInt32(dr["Id_Metodo_Pago"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Activo = dr["Activo"] != DBNull.Value && Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<MetodoPago>> ListarMetodosPagoFiltroAsync(string filtro)
        {
            var olist = new List<MetodoPago>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsMetodosPago", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new MetodoPago
                        {
                            Id_Metodo_Pago = Convert.ToInt32(dr["Id_Metodo_Pago"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Activo = dr["Activo"] != DBNull.Value && Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoMetodoPagoAsync(MetodoPago metodo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsMetodosPago", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", metodo.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", metodo.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Activo", metodo.Activo));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", metodo.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)metodo.Id_Modificador ?? DBNull.Value));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarMetodoPagoAsync(MetodoPago metodo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsMetodosPago", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Metodo_Pago", metodo.Id_Metodo_Pago));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", metodo.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", metodo.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Activo", metodo.Activo));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)metodo.Id_Modificador ?? DBNull.Value));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarMetodoPagoAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsMetodosPago", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Metodo_Pago", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}