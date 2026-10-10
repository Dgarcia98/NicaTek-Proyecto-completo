using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using GRUPO_DDR_INFRASTUCTURE.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GRUPO_DDR_INFRASTRUCTURE.Repositories
{
    public class TipoMovimientoRepository : ITipoMovimientoRepository
    {
        private readonly DbConection _conexion;

        public TipoMovimientoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<TipoMovimiento>> ListarTipoMovimiento()
        {
            var olist = new List<TipoMovimiento>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Movimiento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new TipoMovimiento
                        {
                            Id_Tipo_Movimiento = Convert.ToInt32(dr["Id_Tipo_Movimiento"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            Operacion = dr["Operacion"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Nombre_Estado"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<TipoMovimiento>> ListarPorNombre(string filtro)
        {
            var olist = new List<TipoMovimiento>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Movimiento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new TipoMovimiento
                        {
                            Id_Tipo_Movimiento = Convert.ToInt32(dr["Id_Tipo_Movimiento"]),
                            Descripcion = dr["Descripcion"].ToString(),
                            Operacion = dr["Operacion"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Nombre_Estado"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoTipoMovimiento(TipoMovimiento entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Movimiento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Descripcion", entidad.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Operacion", entidad.Operacion));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador ?? (object)DBNull.Value));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarTipoMovimiento(TipoMovimiento entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Movimiento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Movimiento", entidad.Id_Tipo_Movimiento));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", entidad.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Operacion", entidad.Operacion));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarTipoMovimiento(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Movimiento", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Movimiento", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}