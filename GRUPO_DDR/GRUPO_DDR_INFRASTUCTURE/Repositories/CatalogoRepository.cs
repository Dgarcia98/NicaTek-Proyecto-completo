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
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly DbConection _conexion;

        public CatalogoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Catalogo>> ListarCatalogosAsync()
        {
            var olist = new List<Catalogo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsCatalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Catalogo
                        {
                            Id_Catalogo = Convert.ToInt32(dr["Id_Catalogo"]),
                            Id_Tipo_Catalogo = Convert.ToInt32(dr["Id_Tipo_Catalogo"]),
                            Tipo_Catalogo_Nombre = dr["Tipo_Catalogo"].ToString(),
                            Nombre_Catalogo = dr["Catalogo"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Catalogo>> ListarCatalogosFiltroAsync(string filtro)
        {
            var olist = new List<Catalogo>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsCatalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Catalogo
                        {
                            Id_Catalogo = Convert.ToInt32(dr["Id_Catalogo"]),
                            Id_Tipo_Catalogo = Convert.ToInt32(dr["Id_Tipo_Catalogo"]),
                            Tipo_Catalogo_Nombre = dr["Tipo_Catalogo"].ToString(),
                            Nombre_Catalogo = dr["Catalogo"].ToString(),
                            Activo = Convert.ToBoolean(dr["Activo"]),
                            Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]),
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoCatalogoAsync(Catalogo catalogo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsCatalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Catalogo", catalogo.Id_Tipo_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Catalogo", catalogo.Nombre_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", catalogo.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", catalogo.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", catalogo.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarCatalogoAsync(Catalogo catalogo)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsCatalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Catalogo", catalogo.Id_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Id_Tipo_Catalogo", catalogo.Id_Tipo_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Catalogo", catalogo.Nombre_Catalogo));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", catalogo.Id_Modificador));
                cmd.Parameters.Add(new SqlParameter("@Activo", catalogo.Activo));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarCatalogoAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("spClsCatalogos", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Catalogo", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}