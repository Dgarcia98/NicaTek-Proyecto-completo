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
    public class CorreoRepository : ICorreoRepository
    {
        private readonly DbConection _conexion;

        public CorreoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Correos>> ListarCorreoAsync()
        {
            var olist = new List<Correos>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Correos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Correos
                        {
                            Id_Correo = Convert.ToInt32(dr["Id_Correo"]),
                            Direccion_Correo = dr["Direccion_Correo"].ToString(),
                            Id_Tipo_Correo = dr["Id_Tipo_Correo"] != DBNull.Value ? Convert.ToInt32(dr["Id_Tipo_Correo"]) : 0,
                            Tipo_Correo = dr["Tipo_Correo"].ToString(),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Nombre_Completo = dr["Nombre_Completo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            EstadoActivo = dr["EstadoActivo"].ToString()
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Correos>> ListarCorreoFiltroAsync(string filtro)
        {
            var olist = new List<Correos>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Correos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Correos
                        {
                            Id_Correo = Convert.ToInt32(dr["Id_Correo"]),
                            Direccion_Correo = dr["Direccion_Correo"].ToString(),
                            Id_Tipo_Correo = dr["Id_Tipo_Correo"] != DBNull.Value ? Convert.ToInt32(dr["Id_Tipo_Correo"]) : 0,
                            Tipo_Correo = dr["Tipo_Correo"].ToString(),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Nombre_Completo = dr["Nombre_Completo"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            EstadoActivo = dr["EstadoActivo"].ToString()
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoCorreoAsync(Correos oCorreo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Correos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", oCorreo.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Direccion_Correo", oCorreo.Direccion_Correo));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Correo", oCorreo.Id_Tipo_Correo));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", oCorreo.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oCorreo.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", oCorreo.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarCorreoAsync(Correos oCorreo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Correos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Correo", oCorreo.Id_Correo));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", oCorreo.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Direccion_Correo", oCorreo.Direccion_Correo));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Correo", oCorreo.Id_Tipo_Correo));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oCorreo.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", oCorreo.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarCorreoAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Correos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Correo", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}