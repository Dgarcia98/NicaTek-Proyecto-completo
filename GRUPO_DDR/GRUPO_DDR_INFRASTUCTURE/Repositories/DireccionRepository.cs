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
    public class DireccionRepository : IDireccionRepository
    {
        private readonly DbConection _conexion;

        public DireccionRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Direccion>> ListarDireccionesAsync()
        {
            var olist = new List<Direccion>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Direcciones", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Direccion
                        {
                            Id_Direccion = Convert.ToInt32(dr["Id_Direccion"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Nombre_Completo = dr["Nombre_Completo"].ToString(), // Campo del JOIN
                            Direccion_Completa = dr["Direccion_Completa"].ToString(),
                            Id_Tipo_Direccion = Convert.ToInt32(dr["Id_Tipo_Direccion"]),
                            Tipo_Direccion = dr["Tipo_Direccion"].ToString(), // Campo del JOIN
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            // Nota: Asegúrate de que tu SP 'LST' devuelva estas columnas, si no, usa validación DBNull
                            Id_Creador = dr.GetSchemaTable().Select("ColumnName = 'Id_Creador'").Length > 0 && dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr.GetSchemaTable().Select("ColumnName = 'Id_Modificador'").Length > 0 && dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Direccion>> FiltrarDireccionesAsync(string filtro)
        {
            var olist = new List<Direccion>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Direcciones", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Direccion
                        {
                            Id_Direccion = Convert.ToInt32(dr["Id_Direccion"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Nombre_Completo = dr["Nombre_Completo"].ToString(),
                            Direccion_Completa = dr["Direccion_Completa"].ToString(),
                            Id_Tipo_Direccion = Convert.ToInt32(dr["Id_Tipo_Direccion"]),
                            Tipo_Direccion = dr["Tipo_Direccion"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = dr.GetSchemaTable().Select("ColumnName = 'Id_Creador'").Length > 0 && dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr.GetSchemaTable().Select("ColumnName = 'Id_Modificador'").Length > 0 && dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevaDireccionAsync(Direccion direccion)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Direcciones", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", direccion.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Direccion_Completa", direccion.Direccion_Completa));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Direccion", direccion.Id_Tipo_Direccion));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", direccion.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", direccion.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", direccion.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
             
            }
        }

        public async Task EditarDireccionAsync(Direccion direccion)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Direcciones", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Direccion", direccion.Id_Direccion));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", direccion.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Direccion_Completa", direccion.Direccion_Completa));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Direccion", direccion.Id_Tipo_Direccion));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", direccion.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", direccion.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
              
            }
        }

        public async Task EliminarDireccionAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Direcciones", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Direccion", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
              
            }
        }
    }
}