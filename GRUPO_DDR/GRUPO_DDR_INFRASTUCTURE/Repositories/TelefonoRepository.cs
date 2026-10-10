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
    public class TelefonoRepository : ITelefonoRepository
    {
        private readonly DbConection _conexion;

        public TelefonoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Telefonos>> ListarTelefonosAsync()
        {
            var olist = new List<Telefonos>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Telefonos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Telefonos
                        {
                            Id_Telefono = Convert.ToInt32(dr["Id_Telefono"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Numero_Telefono = dr["Numero_Telefono"].ToString(),
                            Id_Tipo_Telefono = dr["Id_Tipo_Telefono"] != DBNull.Value ? Convert.ToInt32(dr["Id_Tipo_Telefono"]) : (int?)null,
                            Tipo_Telefono = dr["Tipo_Telefono"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null,
                            Estado = Convert.ToBoolean(dr["Activo"]),
                            Nombre_Estado = dr["EstadoActivo"].ToString() 
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Telefonos>> ListarTelefonosFiltroAsync(string filtro)
        {
            var olist = new List<Telefonos>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Telefonos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Telefonos
                        {
                            Id_Telefono = Convert.ToInt32(dr["Id_Telefono"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Numero_Telefono = dr["Numero_Telefono"].ToString(),
                            Id_Tipo_Telefono = dr["Id_Tipo_Telefono"] != DBNull.Value ? Convert.ToInt32(dr["Id_Tipo_Telefono"]) : (int?)null,
                            Tipo_Telefono = dr["Tipo_Telefono"].ToString(),

                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null,

                            Estado = Convert.ToBoolean(dr["Activo"]),
                            Nombre_Estado = dr["EstadoActivo"].ToString()
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoTelefonoAsync(Telefonos oTelefono)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Telefonos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Id_Persona", oTelefono.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Numero_Telefono", oTelefono.Numero_Telefono));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Telefono", (object)oTelefono.Id_Tipo_Telefono ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = oTelefono.Estado });
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", oTelefono.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oTelefono.Id_Creador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarTelefonoAsync(Telefonos oTelefono)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Telefonos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Telefono", oTelefono.Id_Telefono));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", oTelefono.Id_Persona));
                cmd.Parameters.Add(new SqlParameter("@Numero_Telefono", oTelefono.Numero_Telefono));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Telefono", (object)oTelefono.Id_Tipo_Telefono ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = oTelefono.Estado });

                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oTelefono.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarTelefonoAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Telefonos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Telefono", id));
                cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = false });

                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}