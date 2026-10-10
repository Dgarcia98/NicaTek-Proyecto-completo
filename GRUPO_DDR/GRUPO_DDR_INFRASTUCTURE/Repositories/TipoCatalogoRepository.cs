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
    public class TipoCatalogoRepository : ITipoCatalogoRepository
    {
        private readonly DbConection _conexion;

        public TipoCatalogoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<TipoCatalogo>> ListarTipoCatalogo()
        {
            var olist = new List<TipoCatalogo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Catalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new TipoCatalogo
                        {
                            Id_Tipo_Catalogo = Convert.ToInt32(dr["Id_Tipo_Catalogo"]),
                            Tipo_Catalogo = dr["Tipo_Catalogo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0,
                            Estado = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<TipoCatalogo>> ListarPorNombre(string filtro)
        {
            var olist = new List<TipoCatalogo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Catalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new TipoCatalogo
                        {
                            Id_Tipo_Catalogo = Convert.ToInt32(dr["Id_Tipo_Catalogo"]),
                            Tipo_Catalogo = dr["Tipo_Catalogo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0,
                            Estado = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<TipoCatalogo> ObtenerTipoCatalogoPorId(int id)
        {
            TipoCatalogo item = null;

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Catalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", id.ToString()));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        item = new TipoCatalogo
                        {
                            Id_Tipo_Catalogo = Convert.ToInt32(dr["Id_Tipo_Catalogo"]),
                            Tipo_Catalogo = dr["Tipo_Catalogo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0,
                            Estado = Convert.ToBoolean(dr["Activo"])
                        };
                    }
                }
            }
            return item;
        }

        public async Task NuevoTipoCatalogo(TipoCatalogo entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Catalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Tipo_Catalogo", entidad.Tipo_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", entidad.Estado));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarTipoCatalogo(TipoCatalogo entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Catalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Catalogo", entidad.Id_Tipo_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Tipo_Catalogo", entidad.Tipo_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", entidad.Estado));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarTipoCatalogo(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Tipo_Catalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Catalogo", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}