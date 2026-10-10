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
    public class BodegaProductoRepository : IBodegaProductoRepository
    {
        private readonly DbConection _conexion;

        public BodegaProductoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        private BodegaProducto Mapear(SqlDataReader dr) => new BodegaProducto
        {
            Id_Bodega_Producto = Convert.ToInt32(dr["Id_Bodega_Producto"]),
            Id_Bodega = Convert.ToInt32(dr["Id_Bodega"]),
            Nombre_Bodega = dr["Nombre_Bodega"] != DBNull.Value ? dr["Nombre_Bodega"].ToString() : null,
            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
            Codigo_Producto = dr["Codigo_Producto"] != DBNull.Value ? dr["Codigo_Producto"].ToString() : null,
            Nombre_Producto = dr["Nombre_Producto"] != DBNull.Value ? dr["Nombre_Producto"].ToString() : null,
            Precio_Producto = dr["Precio_Producto"] != DBNull.Value ? Convert.ToDecimal(dr["Precio_Producto"]) : 0,
            Id_Marca = dr["Id_Marca"] != DBNull.Value ? Convert.ToInt32(dr["Id_Marca"]) : 0,
            Nombre_Marca = dr["Nombre_Marca"] != DBNull.Value ? dr["Nombre_Marca"].ToString() : null,
            Id_Modelo = dr["Id_Modelo"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modelo"]) : 0,
            Nombre_Modelo = dr["Nombre_Modelo"] != DBNull.Value ? dr["Nombre_Modelo"].ToString() : null,
            Nombre_Categoria = dr["Nombre_Categoria"] != DBNull.Value ? dr["Nombre_Categoria"].ToString() : null,
            Cantidad = Convert.ToInt32(dr["Cantidad"]),
            Estado_Bodega = dr["Estado_Bodega"] != DBNull.Value ? dr["Estado_Bodega"].ToString() : null,
            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
        };

        public async Task<IEnumerable<BodegaProducto>> ListarAsync()
        {
            var lista = new List<BodegaProducto>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega_Producto", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) lista.Add(Mapear(dr));
            return lista;
        }

        public async Task<IEnumerable<BodegaProducto>> FiltrarAsync(string filtro)
        {
            var lista = new List<BodegaProducto>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega_Producto", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
            cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) lista.Add(Mapear(dr));
            return lista;
        }

        public async Task<IEnumerable<BodegaProducto>> ListarPorBodegaAsync(int idBodega)
        {
            var lista = new List<BodegaProducto>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega_Producto", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "PBD"));
            cmd.Parameters.Add(new SqlParameter("@Id_Bodega", idBodega));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) lista.Add(Mapear(dr));
            return lista;
        }

        public async Task NuevoAsync(BodegaProducto entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega_Producto", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
            cmd.Parameters.Add(new SqlParameter("@Id_Bodega", entidad.Id_Bodega));
            cmd.Parameters.Add(new SqlParameter("@Id_Producto", entidad.Id_Producto));
            cmd.Parameters.Add(new SqlParameter("@Cantidad", entidad.Cantidad));
            cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
            cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));
            cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EditarAsync(BodegaProducto entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega_Producto", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
            cmd.Parameters.Add(new SqlParameter("@Id_Bodega_Producto", entidad.Id_Bodega_Producto));
            cmd.Parameters.Add(new SqlParameter("@Cantidad", entidad.Cantidad));
            cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));
            cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega_Producto", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
            cmd.Parameters.Add(new SqlParameter("@Id_Bodega_Producto", id));
            cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));
            cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });
            await cmd.ExecuteNonQueryAsync();
        }
    }
}