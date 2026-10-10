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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DbConection _conexion;

        public UsuarioRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<Usuario> Login(string usuario)
        {
            Usuario user = null;
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("spLogin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Usuario", usuario);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                user = new Usuario();
                user.Id_Usuario = Convert.ToInt32(dr["Id_Usuario"]);
                user.Nombre_Usuario = dr["Nombre_Usuario"].ToString();
                user.Contrasena = dr["Contrasena"].ToString(); // Hash recuperado para validar en Service
                user.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);
                user.Nombre_Rol = dr["Nombre_Rol"].ToString();
                user.Id_Persona = Convert.ToInt32(dr["Id_Persona"]);
                user.Nombre_Persona = dr["Nombre_Completo"].ToString();

                user.Fecha_Ultimo_Acceso = dr["Fecha_Ultimo_Acceso"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(dr["Fecha_Ultimo_Acceso"]);
                user.Estado = true;
            }
            return user;
        }


        public async Task<IEnumerable<string>> ObtenerPermisos(int idUsuario)
        {
            var permisos = new List<string>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("sp_Usuario_Permisos", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Id_Usuario", SqlDbType.Int).Value = idUsuario;

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                var codigo = dr["Codigo_Permiso"]?.ToString();
                if (!string.IsNullOrWhiteSpace(codigo))
                    permisos.Add(codigo);
            }

            return permisos;
        }

        public async Task<IEnumerable<Usuario>> ListarUsuarios()
        {
            var olist = new List<Usuario>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("spTblUsuarios", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "LST");

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                var entidad = new Usuario();
                entidad.Id_Usuario = Convert.ToInt32(dr["Id_Usuario"]);
                entidad.Nombre_Usuario = dr["Nombre_Usuario"].ToString();
                entidad.Id_Persona = Convert.ToInt32(dr["Id_Persona"]);
                entidad.Nombre_Persona = dr["Nombre_Persona"].ToString(); 
                entidad.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);
                entidad.Nombre_Rol = dr["Nombre_Rol"].ToString();
                entidad.Id_Estado = Convert.ToInt32(dr["Id_Estado"]);
                entidad.Estado_Nombre = dr["Estado"].ToString();
                entidad.Estado = entidad.Id_Estado == 1;

                entidad.Fecha_Ultimo_Acceso = dr["Fecha_Ultimo_Acceso"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["Fecha_Ultimo_Acceso"]);
                entidad.Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]);
                entidad.Id_Creador = Convert.ToInt32(dr["Id_Creador"]);

                olist.Add(entidad);
            }
            return olist;
        }

        public async Task<IEnumerable<Usuario>> ListarPorFiltro(string filtro)
        {
            var olist = new List<Usuario>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("spTblUsuarios", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "FIL");
            cmd.Parameters.AddWithValue("@Filtro", filtro);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                var entidad = new Usuario();
                entidad.Id_Usuario = Convert.ToInt32(dr["Id_Usuario"]);
                entidad.Nombre_Usuario = dr["Nombre_Usuario"].ToString();
                entidad.Id_Persona = Convert.ToInt32(dr["Id_Persona"]);
                entidad.Nombre_Persona = dr["Nombre_Persona"].ToString();
                entidad.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);
                entidad.Id_Estado = Convert.ToInt32(dr["Id_Estado"]);
                entidad.Estado = entidad.Id_Estado == 1;

                entidad.Fecha_Creacion = Convert.ToDateTime(dr["Fecha_Creacion"]);
                entidad.Id_Creador = Convert.ToInt32(dr["Id_Creador"]);

                olist.Add(entidad);
            }
            return olist;
        }

        public async Task NuevoUsuario(Usuario entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("spTblUsuarios", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "INS");

            cmd.Parameters.AddWithValue("@Nombre_Usuario", entidad.Nombre_Usuario);
            cmd.Parameters.AddWithValue("@Contrasena", entidad.Contrasena);
            cmd.Parameters.AddWithValue("@Id_Persona", entidad.Id_Persona);
            cmd.Parameters.AddWithValue("@Id_Rol", entidad.Id_Rol);
            cmd.Parameters.AddWithValue("@Id_Estado", entidad.Id_Estado);
            cmd.Parameters.AddWithValue("@Id_Creador", entidad.Id_Creador);

            var oMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            var oNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(oMsg);
            cmd.Parameters.Add(oNum);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EditarUsuario(Usuario entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("spTblUsuarios", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "UPD");

            cmd.Parameters.AddWithValue("@Id_Usuario", entidad.Id_Usuario);
            cmd.Parameters.AddWithValue("@Nombre_Usuario", entidad.Nombre_Usuario);
            cmd.Parameters.AddWithValue("@Contrasena", entidad.Contrasena);
            cmd.Parameters.AddWithValue("@Id_Persona", entidad.Id_Persona);
            cmd.Parameters.AddWithValue("@Id_Rol", entidad.Id_Rol);
            cmd.Parameters.AddWithValue("@Id_Estado", entidad.Id_Estado);
            cmd.Parameters.AddWithValue("@Id_Modificador", entidad.Id_Modificador);

            var oMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            var oNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(oMsg);
            cmd.Parameters.Add(oNum);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarUsuario(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using SqlCommand cmd = new SqlCommand("spTblUsuarios", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Mode", "DEL");
            cmd.Parameters.AddWithValue("@Id_Usuario", id);
            cmd.Parameters.AddWithValue("@Id_Modificador", idModificador);

            var oMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            var oNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(oMsg);
            cmd.Parameters.Add(oNum);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}