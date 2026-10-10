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
    public class RolRepository : IRolRepository
    {
        private readonly DbConection _conexion;

        public RolRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Rol>> ListarRolesAsync()
        {
            var olist = new List<Rol>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Roles", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Rol
                        {
                            Id_Rol = Convert.ToInt32(dr["Id_Rol"]),
                            Nombre_Rol = dr["Nombre_Rol"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null,
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            EstadoActivo = dr["EstadoActivo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : null,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Rol>> ListarRolesFiltroAsync(string filtro)
        {
            var olist = new List<Rol>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Roles", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Rol
                        {
                            Id_Rol = Convert.ToInt32(dr["Id_Rol"]),
                            Nombre_Rol = dr["Nombre_Rol"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : null,
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            EstadoActivo = dr["EstadoActivo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : null,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoRolAsync(Rol rol)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Roles", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Nombre_Rol", rol.Nombre_Rol));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(rol.Descripcion) ? DBNull.Value : rol.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", rol.Id_Creador ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", rol.Id_Modificador ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Activo", rol.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();

            }
        }

        public async Task EditarRolAsync(Rol rol)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Roles", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Rol", rol.Id_Rol));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Rol", rol.Nombre_Rol));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", string.IsNullOrEmpty(rol.Descripcion) ? DBNull.Value : rol.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", rol.Id_Modificador ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Activo", rol.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarRolAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Roles", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Rol", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}