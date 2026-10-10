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
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly DbConection _conexion;

        public ProveedorRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Proveedores>> ListarProveedoresAsync()
        {
            var olist = new List<Proveedores>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Proveedor", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Proveedores
                        {
                            Id_Proveedor = Convert.ToInt32(dr["Id_Proveedor"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Cedula = dr["Cedula"].ToString(),
                            Primer_Nombre = dr["Primer_Nombre"].ToString(),
                            Segundo_Nombre = dr["Segundo_Nombre"] != DBNull.Value ? dr["Segundo_Nombre"].ToString() : null,
                            Primer_Apellido = dr["Primer_Apellido"].ToString(),
                            Segundo_Apellido = dr["Segundo_Apellido"] != DBNull.Value ? dr["Segundo_Apellido"].ToString() : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "",
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Fecha_Terminacion_Contrato = dr["Fecha_Terminacion_Contrato"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Terminacion_Contrato"]) : (DateTime?)null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Proveedores>> ListarProveedoresNombreAsync(string filtro)
        {
            var olist = new List<Proveedores>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Proveedor", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Proveedores
                        {
                            Id_Proveedor = Convert.ToInt32(dr["Id_Proveedor"]),
                            Id_Persona = Convert.ToInt32(dr["Id_Persona"]),
                            Cedula = dr["Cedula"].ToString(),
                            Primer_Nombre = dr["Primer_Nombre"].ToString(),
                            Segundo_Nombre = dr["Segundo_Nombre"] != DBNull.Value ? dr["Segundo_Nombre"].ToString() : null,
                            Primer_Apellido = dr["Primer_Apellido"].ToString(),
                            Segundo_Apellido = dr["Segundo_Apellido"] != DBNull.Value ? dr["Segundo_Apellido"].ToString() : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : "",
                            FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                            Fecha_Terminacion_Contrato = dr["Fecha_Terminacion_Contrato"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Terminacion_Contrato"]) : (DateTime?)null,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : (int?)null
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoProveedoresAsync(Proveedores oProveedor)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Proveedor", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", oProveedor.Id_Persona)); 
                cmd.Parameters.Add(new SqlParameter("@Cedula", oProveedor.Cedula));
                cmd.Parameters.Add(new SqlParameter("@Primer_Nombre", oProveedor.Primer_Nombre));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Nombre", (object)oProveedor.Segundo_Nombre ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Primer_Apellido", oProveedor.Primer_Apellido));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Apellido", (object)oProveedor.Segundo_Apellido ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@FechaRegistro", oProveedor.FechaRegistro));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Terminacion_Contrato", (object)oProveedor.Fecha_Terminacion_Contrato ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oProveedor.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", oProveedor.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)oProveedor.Id_Modificador ?? DBNull.Value));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarProveedoresAsync(Proveedores oProveedor)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Proveedor", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Proveedor", oProveedor.Id_Proveedor));
                cmd.Parameters.Add(new SqlParameter("@Id_Persona", oProveedor.Id_Persona)); 
                cmd.Parameters.Add(new SqlParameter("@Cedula", oProveedor.Cedula));
                cmd.Parameters.Add(new SqlParameter("@Primer_Nombre", oProveedor.Primer_Nombre));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Nombre", (object)oProveedor.Segundo_Nombre ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Primer_Apellido", oProveedor.Primer_Apellido));
                cmd.Parameters.Add(new SqlParameter("@Segundo_Apellido", (object)oProveedor.Segundo_Apellido ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@FechaRegistro", oProveedor.FechaRegistro));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Terminacion_Contrato", (object)oProveedor.Fecha_Terminacion_Contrato ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oProveedor.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", (object)oProveedor.Id_Modificador ?? DBNull.Value));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarProveedoresAsync(int id)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Proveedor", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Proveedor", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", 1));

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}