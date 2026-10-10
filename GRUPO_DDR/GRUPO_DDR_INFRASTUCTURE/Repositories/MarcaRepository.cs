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
    public class MarcaRepository : IMarcaRepository
    {
        private readonly DbConection _conexion;

        public MarcaRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Marca>> ListarMarcaAsync()
        {
            var olist = new List<Marca>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Marca", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Marca
                        {
                            Id_Marca = Convert.ToInt32(dr["Id_Marca"]),
                            Nombre = dr["Nombre_Marca"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Marca>> ListarMarcaFiltroAsync(string filtro)
        {
            var olist = new List<Marca>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Marca", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Marca
                        {
                            Id_Marca = Convert.ToInt32(dr["Id_Marca"]),
                            Nombre = dr["Nombre_Marca"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null,
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevaMarcaAsync(Marca oMarca)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Marca", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Nombre_Marca", oMarca.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oMarca.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", oMarca.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oMarca.Id_Creador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarMarcaAsync(Marca oMarca)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Marca", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Marca", oMarca.Id_Marca));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Marca", oMarca.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oMarca.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oMarca.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarMarcaAsync(int id)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Marca", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Marca", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", 1));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}