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
    public class DatosPersonalesRepository : IDatosPersonalesRepository
    {
        private readonly DbConection _conexion;

        public DatosPersonalesRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<DatosPersonales>> ObtenerTodosAsync()
        {
            var olist = new List<DatosPersonales>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Datos_Personales", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DatosPersonales
                        {
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Primer_Nombre = dr["Primer_Nombre"].ToString(),
                            Segundo_Nombre = dr["Segundo_Nombre"] != DBNull.Value ? dr["Segundo_Nombre"].ToString() : null,
                            Primer_Apellido = dr["Primer_Apellido"].ToString(),
                            Segundo_Apellido = dr["Segundo_Apellido"] != DBNull.Value ? dr["Segundo_Apellido"].ToString() : null,
                            Id_Tipo_DNI = Convert.ToInt32(dr["Id_Tipo_DNI"]),
                            DNI = dr["DNI"].ToString(),
                            Id_Genero = Convert.ToInt32(dr["Id_Genero"]),
                            Id_Nacionalidad = Convert.ToInt32(dr["Id_Nacionalidad"]),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null,
                            Estado = Convert.ToInt32(dr["Id_Estado"]) == 1
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<DatosPersonales>> ObtenerPorFiltroAsync(string filtro)
        {
            var olist = new List<DatosPersonales>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Datos_Personales", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new DatosPersonales
                        {
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Primer_Nombre = dr["Primer_Nombre"].ToString(),
                            Segundo_Nombre = dr["Segundo_Nombre"] != DBNull.Value ? dr["Segundo_Nombre"].ToString() : null,
                            Primer_Apellido = dr["Primer_Apellido"].ToString(),
                            Segundo_Apellido = dr["Segundo_Apellido"] != DBNull.Value ? dr["Segundo_Apellido"].ToString() : null,
                            Id_Tipo_DNI = Convert.ToInt32(dr["Id_Tipo_DNI"]),
                            DNI = dr["DNI"].ToString(),
                            Id_Genero = Convert.ToInt32(dr["Id_Genero"]),
                            Id_Nacionalidad = Convert.ToInt32(dr["Id_Nacionalidad"]),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null,
                            Estado = Convert.ToInt32(dr["Id_Estado"]) == 1
                        });
                    }
                }
            }
            return olist;
        }

        public async Task InsertarAsync(DatosPersonales entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Datos_Personales", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Primer_Nombre", entidad.Primer_Nombre));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Nombre", (object)entidad.Segundo_Nombre ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Primer_Apellido", entidad.Primer_Apellido));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Apellido", (object)entidad.Segundo_Apellido ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_DNI", entidad.Id_Tipo_DNI));
                cmd.Parameters.Add(new SqlParameter("@DNI", entidad.DNI));
                cmd.Parameters.Add(new SqlParameter("@Id_Genero", entidad.Id_Genero));
                cmd.Parameters.Add(new SqlParameter("@Id_Nacionalidad", entidad.Id_Nacionalidad));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)entidad.Id_Modificador ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Estado ? 1 : 2));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task ActualizarAsync(DatosPersonales entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Datos_Personales", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Persona", entidad.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Primer_Nombre", entidad.Primer_Nombre));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Nombre", (object)entidad.Segundo_Nombre ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Primer_Apellido", entidad.Primer_Apellido));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Apellido", (object)entidad.Segundo_Apellido ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_DNI", entidad.Id_Tipo_DNI));
                cmd.Parameters.Add(new SqlParameter("@DNI", entidad.DNI));
                cmd.Parameters.Add(new SqlParameter("@Id_Genero", entidad.Id_Genero));
                cmd.Parameters.Add(new SqlParameter("@Id_Nacionalidad", entidad.Id_Nacionalidad));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)entidad.Id_Modificador ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Estado ? 1 : 2));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarAsync(int idPersona)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Datos_Personales", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", idPersona));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", 1));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}