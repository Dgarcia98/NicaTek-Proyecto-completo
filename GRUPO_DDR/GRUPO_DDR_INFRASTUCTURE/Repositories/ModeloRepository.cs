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
    public class ModeloRepository : IModeloRepository
    {
        private readonly DbConection _conexion;

        public ModeloRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Modelo>> ListarModeloAsync()
        {
            var olist = new List<Modelo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Modelo
                        {
                            Id_Modelo = Convert.ToInt32(dr["Id_Modelo"]),
                            Nombre = dr["Nombre_Modelo"].ToString(),
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

        public async Task<IEnumerable<Modelo>> ListarModeloFiltroAsync(string filtro)
        {
            var olist = new List<Modelo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Modelo
                        {
                            Id_Modelo = Convert.ToInt32(dr["Id_Modelo"]),
                            Nombre = dr["Nombre_Modelo"].ToString(),
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

        public async Task NuevaModeloAsync(Modelo oModelo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Modelo", oModelo.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oModelo.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", oModelo.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oModelo.Id_Creador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarModeloAsync(Modelo oModelo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Modelo", oModelo.Id_Modelo));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Modelo", oModelo.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oModelo.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oModelo.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarModeloAsync(int id)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Cls_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Modelo", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", 1)); 

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}