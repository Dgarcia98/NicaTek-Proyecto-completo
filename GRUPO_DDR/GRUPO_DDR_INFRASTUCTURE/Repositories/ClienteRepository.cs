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
    public class ClienteRepository : IClienteRepository
    {
        private readonly DbConection _conexion;

        public ClienteRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Cliente>> ListarClientes()
        {
            var olist = new List<Cliente>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using SqlCommand cmd = new SqlCommand("sp_Tbl_Cliente", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "LST");

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                var entidad = new Cliente();
                entidad.Id_Cliente = Convert.ToInt32(dr["Id_Cliente"]);
                entidad.Id_Persona = Convert.ToInt32(dr["Id_Persona"]);
                entidad.Nombre_Completo = dr["Nombre_Completo"].ToString();
                entidad.Id_Estado = Convert.ToInt32(dr["Id_Estado"]);
                entidad.Nombre_Estado = dr["Estado"].ToString();
                entidad.Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]);
                entidad.Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]);
                entidad.Id_Creador = Convert.ToInt32(dr["Id_Creador"]);
                entidad.Id_Modificador = Convert.ToInt32(dr["Id_Modificador"]);

                olist.Add(entidad);
            }
            return olist;
        }

        public async Task<IEnumerable<Cliente>> ListarPorFiltro(string filtro)
        {
            var olist = new List<Cliente>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using SqlCommand cmd = new SqlCommand("sp_Tbl_Cliente", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "FIL");
            cmd.Parameters.AddWithValue("@Filtro", filtro);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                var entidad = new Cliente();
                entidad.Id_Cliente = Convert.ToInt32(dr["Id_Cliente"]);
                entidad.Id_Persona = Convert.ToInt32(dr["Id_Persona"]);
                entidad.Nombre_Completo = dr["Nombre_Completo"].ToString();
                entidad.Id_Estado = Convert.ToInt32(dr["Id_Estado"]);
                entidad.Nombre_Estado = dr["Estado"].ToString();
                entidad.Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]);
                entidad.Fecha_Modificacion = Convert.ToDateTime(dr["Fecha_Modificacion"]);
                entidad.Id_Creador = Convert.ToInt32(dr["Id_Creador"]);
                entidad.Id_Modificador = Convert.ToInt32(dr["Id_Modificador"]);

                olist.Add(entidad);
            }
            return olist;
        }

        public async Task NuevoCliente(Cliente entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using SqlCommand cmd = new SqlCommand("sp_Tbl_Cliente", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "INS");
            cmd.Parameters.AddWithValue("@Id_Persona", entidad.Id_Persona);
            cmd.Parameters.AddWithValue("@Id_Estado", entidad.Id_Estado);
            cmd.Parameters.AddWithValue("@Id_Creador", entidad.Id_Creador);
            cmd.Parameters.AddWithValue("@Id_Modificador", entidad.Id_Modificador);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EditarCliente(Cliente entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using SqlCommand cmd = new SqlCommand("sp_Tbl_Cliente", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "UPD");
            cmd.Parameters.AddWithValue("@Id_Cliente", entidad.Id_Cliente);
            cmd.Parameters.AddWithValue("@Id_Persona", entidad.Id_Persona);
            cmd.Parameters.AddWithValue("@Id_Estado", entidad.Id_Estado);
            cmd.Parameters.AddWithValue("@Id_Modificador", entidad.Id_Modificador);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarCliente(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using SqlCommand cmd = new SqlCommand("sp_Tbl_Cliente", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "DEL");
            cmd.Parameters.AddWithValue("@Id_Cliente", id);
            cmd.Parameters.AddWithValue("@Id_Modificador", idModificador);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}