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
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly DbConection _conexion;

        public CategoriaRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Categoria>> ListarCategoriasAsync()
        {
            var olist = new List<Categoria>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spTblCategoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Categoria
                        {
                            Id_Categoria = Convert.ToInt32(dr["Id_Categoria"]),
                            Nombre = dr["Nombre"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Categoria>> ListarCategoriasFiltroAsync(string filtro)
        {
            var olist = new List<Categoria>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spTblCategoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Categoria
                        {
                            Id_Categoria = Convert.ToInt32(dr["Id_Categoria"]),
                            Nombre = dr["Nombre"].ToString(),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Nombre_Estado = dr["Estado"].ToString(),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Id_Creador = Convert.ToInt32(dr["Id_Creador"]),
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoCategoriaAsync(Categoria categoria)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spTblCategoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Nombre", categoria.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", categoria.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", categoria.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", categoria.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarCategoriaAsync(Categoria categoria)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync(); // ← Solo una vez

            using (SqlCommand cmd = new SqlCommand("spTblCategoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Categoria", categoria.Id_Categoria));
                cmd.Parameters.Add(new SqlParameter("@Nombre", categoria.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", categoria.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", categoria.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                // ← Se eliminó el segundo await con.OpenAsync() que causaba el error 500
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarCategoriaAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync(); // ← Solo una vez

            using (SqlCommand cmd = new SqlCommand("spTblCategoria", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Categoria", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));
                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                // ← Se eliminó el segundo await con.OpenAsync() que causaba el error 500
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}