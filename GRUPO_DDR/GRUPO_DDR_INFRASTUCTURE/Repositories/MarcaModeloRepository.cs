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
    public class MarcaModeloRepository : IMarcaModeloRepository
    {
        private readonly DbConection _conexion;

        public MarcaModeloRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<MarcaModelo>> ListarMarcaModeloAsync()
        {
            var olist = new List<MarcaModelo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Marca_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new MarcaModelo
                        {
                            Id_Marca_Modelo = Convert.ToInt32(dr["Id_Marca_Modelo"]),
                            Id_Marca = Convert.ToInt32(dr["Id_Marca"]),
                            Nombre_Marca = dr["Nombre_Marca"].ToString(),
                            Id_Modelo = Convert.ToInt32(dr["Id_Modelo"]),
                            Nombre_Modelo = dr["Nombre_Modelo"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<MarcaModelo>> ListarMarcaModeloFiltroAsync(string filtro)
        {
            var olist = new List<MarcaModelo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Marca_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new MarcaModelo
                        {
                            Id_Marca_Modelo = Convert.ToInt32(dr["Id_Marca_Modelo"]),
                            Id_Marca = Convert.ToInt32(dr["Id_Marca"]),
                            Nombre_Marca = dr["Nombre_Marca"].ToString(),
                            Id_Modelo = Convert.ToInt32(dr["Id_Modelo"]),
                            Nombre_Modelo = dr["Nombre_Modelo"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : (DateTime?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevaMarcaModeloAsync(MarcaModelo entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Marca_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));

                cmd.Parameters.Add(new SqlParameter("@Id_Marca", entidad.Id_Marca));
                cmd.Parameters.Add(new SqlParameter("@Id_Modelo", entidad.Id_Modelo));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Creador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarMarcaModeloAsync(MarcaModelo entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Marca_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));

                cmd.Parameters.Add(new SqlParameter("@Id_Marca_Modelo", entidad.Id_Marca_Modelo));
                cmd.Parameters.Add(new SqlParameter("@Id_Marca", entidad.Id_Marca));
                cmd.Parameters.Add(new SqlParameter("@Id_Modelo", entidad.Id_Modelo));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarMarcaModeloAsync(int id)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Marca_Modelo", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));

                cmd.Parameters.Add(new SqlParameter("@Id_Marca_Modelo", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", 1));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}