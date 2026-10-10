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
    public class PermisoRepository : IPermisoRepository
    {
        private readonly DbConection _conexion;

        public PermisoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Permiso>> ListarPermisosAsync()
        {
            var olist = new List<Permiso>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Permiso
                        {
                            Id_Permiso = Convert.ToInt32(dr["Id_Permiso"]),
                            Nombre_Permiso = dr["Nombre_Permiso"].ToString(),
                            Codigo_Permiso = dr["Codigo_Permiso"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"])
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<Permiso> ObtenerPermisoPorIdAsync(int id)
        {
            Permiso permiso = null;

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "GET"));
                cmd.Parameters.Add(new SqlParameter("@Id_Permiso", id));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        permiso = new Permiso
                        {
                            Id_Permiso = Convert.ToInt32(dr["Id_Permiso"]),
                            Nombre_Permiso = dr["Nombre_Permiso"].ToString(),
                            Codigo_Permiso = dr["Codigo_Permiso"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"])
                        };
                    }
                }
            }
            return permiso;
        }

        public async Task<IEnumerable<Permiso>> FiltrarPermisosAsync(string filtro)
        {
            var olist = new List<Permiso>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FLT"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Permiso
                        {
                            Id_Permiso = Convert.ToInt32(dr["Id_Permiso"]),
                            Nombre_Permiso = dr["Nombre_Permiso"].ToString(),
                            Codigo_Permiso = dr["Codigo_Permiso"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"])
                        });
                    }
                }
            }
            return olist;
        }

        public async Task InsertarPermisoAsync(Permiso permiso)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Permiso", permiso.Nombre_Permiso));
                cmd.Parameters.Add(new SqlParameter("@Codigo_Permiso", permiso.Codigo_Permiso));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(permiso.Descripcion) ? DBNull.Value : permiso.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", permiso.Id_Estado));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
                int oNum = Convert.ToInt32(cmd.Parameters["@O_Num"].Value);
                if (oNum == -1)
                {
                    throw new Exception(cmd.Parameters["@O_Msg"].Value.ToString());
                }
            }
        }

        public async Task ActualizarPermisoAsync(Permiso permiso)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Permiso", permiso.Id_Permiso));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Permiso", permiso.Nombre_Permiso));
                cmd.Parameters.Add(new SqlParameter("@Codigo_Permiso", permiso.Codigo_Permiso));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(permiso.Descripcion) ? DBNull.Value : permiso.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", permiso.Id_Estado));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
                int oNum = Convert.ToInt32(cmd.Parameters["@O_Num"].Value);
                if (oNum == -1)
                {
                    throw new Exception(cmd.Parameters["@O_Msg"].Value.ToString());
                }
            }
        }

        public async Task EliminarPermisoAsync(int id)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Permiso", id));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();

                int oNum = Convert.ToInt32(cmd.Parameters["@O_Num"].Value);
                if (oNum == -1)
                {
                    throw new Exception(cmd.Parameters["@O_Msg"].Value.ToString());
                }
            }
        }
    }
}