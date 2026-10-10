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
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly DbConection _conexion;

        public EmpleadoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Empleado>> ListarEmpleadosAsync()
        {
            var olist = new List<Empleado>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Empleado", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Empleado
                        {
                            Id_Empleado = Convert.ToInt32(dr["Id_Empleado"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Nombre_Completo = dr["Nombre_Completo"].ToString(),
                            Cedula = dr["Cedula"].ToString(),
                            Fecha_Contratacion = Convert.ToDateTime(dr["Fecha_Contratacion"]),
                            Fecha_Despido = dr["Fecha_Despido"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["Fecha_Despido"]) : null,
                            Id_Cargo = Convert.ToInt32(dr["Id_Cargo"]),
                            Cargo = dr["Cargo"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["Fecha_Modificacion"]) : null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? (int?)Convert.ToInt32(dr["Id_Modificador"]) : null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Empleado>> FiltrarEmpleadosAsync(string filtro)
        {
            var olist = new List<Empleado>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Empleado", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Empleado
                        {
                            Id_Empleado = Convert.ToInt32(dr["Id_Empleado"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Nombre_Completo = dr["Nombre_Completo"].ToString(),
                            Cedula = dr["Cedula"].ToString(),
                            Fecha_Contratacion = Convert.ToDateTime(dr["Fecha_Contratacion"]),
                            Fecha_Despido = dr["Fecha_Despido"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["Fecha_Despido"]) : null,
                            Id_Cargo = Convert.ToInt32(dr["Id_Cargo"]),
                            Cargo = dr["Cargo"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["Fecha_Modificacion"]) : null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? (int?)Convert.ToInt32(dr["Id_Modificador"]) : null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoEmpleadoAsync(Empleado entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Empleado", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Id_Persona", entidad.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Contratacion", entidad.Fecha_Contratacion));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Despido", (object)entidad.Fecha_Despido ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Cargo", entidad.Id_Cargo));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarEmpleadoAsync(Empleado entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Empleado", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Empleado", entidad.Id_Empleado));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", entidad.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Contratacion", entidad.Fecha_Contratacion));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Despido", (object)entidad.Fecha_Despido ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Cargo", entidad.Id_Cargo));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarEmpleadoAsync(int idEmpleado, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Empleado", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Empleado", idEmpleado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}