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
    public class RolPermisoRepository : IRolPermisoRepository
    {
        private readonly DbConection _conexion;

        public RolPermisoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<RolPermiso>> ListarPermisosPorRolAsync(int idRol)
        {
            var olist = new List<RolPermiso>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Roles_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));
                cmd.Parameters.Add(new SqlParameter("@Id_Rol", idRol));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new RolPermiso
                        {
                            Id_Rol_Permiso = Convert.ToInt32(dr["Id_Rol_Permiso"]),
                            Id_Rol = Convert.ToInt32(dr["Id_Rol"]),
                            Nombre_Rol = dr["Nombre_Rol"].ToString(),
                            Id_Permiso = Convert.ToInt32(dr["Id_Permiso"]),
                            Nombre_Permiso = dr["Nombre_Permiso"].ToString(),
                            Codigo_Permiso = dr["Codigo_Permiso"].ToString(),
                            Descripcion_Permiso = dr["Descripcion_Permiso"] != DBNull.Value ? dr["Descripcion_Permiso"].ToString() : null,
                            Activo = Convert.ToBoolean(dr["Activo"])
                        });
                    }
                }
            }
            return olist;
        }

        public async Task AsignarPermisoAsync(int idRol, int idPermiso)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Roles_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Id_Rol", idRol));
                cmd.Parameters.Add(new SqlParameter("@Id_Permiso", idPermiso));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task RemoverPermisoAsync(int idRol, int idPermiso)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Roles_Permisos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Rol", idRol));
                cmd.Parameters.Add(new SqlParameter("@Id_Permiso", idPermiso));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}