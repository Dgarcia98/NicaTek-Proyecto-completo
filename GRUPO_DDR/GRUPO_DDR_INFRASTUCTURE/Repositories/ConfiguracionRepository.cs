using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using GRUPO_DDR_INFRASTRUCTURE.Data;
using GRUPO_DDR_INFRASTUCTURE.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace GRUPO_DDR_INFRASTRUCTURE.Repositories
{
    public class ConfiguracionRepository : IConfiguracionRepository
    {
        private readonly DbConection _conexion;

        public ConfiguracionRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<ConfiguracionEmpresa> ObtenerConfiguracionAsync()
        {
            ConfiguracionEmpresa config = null;
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Configuracion_Empresa", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "GET"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        config = new ConfiguracionEmpresa
                        {
                            Id_Configuracion = Convert.ToInt32(dr["Id_Configuracion"]),
                            Nombre_Empresa = dr["Nombre_Empresa"].ToString(),
                            RUC_No = dr["RUC_No"].ToString(),
                            Direccion = dr["Direccion"].ToString(),
                            Telefono = dr["Telefono"] != DBNull.Value ? dr["Telefono"].ToString() : null,
                            Correo_Empresa = dr["Correo_Empresa"] != DBNull.Value ? dr["Correo_Empresa"].ToString() : null,
                            Ruta_Logo = dr["Ruta_Logo"] != DBNull.Value ? dr["Ruta_Logo"].ToString() : null,
                            Moneda_Simbolo = dr["Moneda_Simbolo"].ToString(),
                            Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"])
                        };
                    }
                }
            }
            return config;
        }

        public async Task InsertarConfiguracionAsync(ConfiguracionEmpresa config)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Configuracion_Empresa", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Empresa", config.Nombre_Empresa));
                cmd.Parameters.Add(new SqlParameter("@RUC_No", config.RUC_No));
                cmd.Parameters.Add(new SqlParameter("@Direccion", config.Direccion));
                cmd.Parameters.Add(new SqlParameter("@Telefono", (object)config.Telefono ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Correo_Empresa", (object)config.Correo_Empresa ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Ruta_Logo", (object)config.Ruta_Logo ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Moneda_Simbolo", config.Moneda_Simbolo));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)config.Id_Modificador ?? DBNull.Value));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();

                if (Convert.ToInt32(cmd.Parameters["@O_Num"].Value) == -1)
                {
                    throw new Exception(cmd.Parameters["@O_Msg"].Value.ToString());
                }
            }
        }

        public async Task ActualizarConfiguracionAsync(ConfiguracionEmpresa config)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Configuracion_Empresa", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Configuracion", config.Id_Configuracion));
                cmd.Parameters.Add(new SqlParameter("@Nombre_Empresa", config.Nombre_Empresa));
                cmd.Parameters.Add(new SqlParameter("@RUC_No", config.RUC_No));
                cmd.Parameters.Add(new SqlParameter("@Direccion", config.Direccion));
                cmd.Parameters.Add(new SqlParameter("@Telefono", (object)config.Telefono ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Correo_Empresa", (object)config.Correo_Empresa ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Ruta_Logo", (object)config.Ruta_Logo ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Moneda_Simbolo", config.Moneda_Simbolo));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)config.Id_Modificador ?? DBNull.Value));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();

                if (Convert.ToInt32(cmd.Parameters["@O_Num"].Value) == -1)
                {
                    throw new Exception(cmd.Parameters["@O_Msg"].Value.ToString());
                }
            }
        }
    }
}