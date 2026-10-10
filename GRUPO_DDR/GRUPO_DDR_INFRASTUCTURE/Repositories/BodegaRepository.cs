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
    public class BodegaRepository : IBodegaRepository
    {
        private readonly DbConection _conexion;

        public BodegaRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        private Bodegas MapearBodega(SqlDataReader dr) => new Bodegas
        {
            Id_Bodega = Convert.ToInt32(dr["Id_Bodega"]),
            Nombre_Bodega = dr["Nombre_Bodega"] != DBNull.Value ? dr["Nombre_Bodega"].ToString() : null,
            Cantidad = Convert.ToInt32(dr["Stock_Total"]),
            Total_Productos = dr["Total_Productos"] != DBNull.Value ? Convert.ToInt32(dr["Total_Productos"]) : 0,
            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
            Nombre_Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : null,
            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0
        };

        public async Task<IEnumerable<Bodegas>> ListarBodegaAsync()
        {
            var lista = new List<Bodegas>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) lista.Add(MapearBodega(dr));
            return lista;
        }

        public async Task<IEnumerable<Bodegas>> ListarBodegaFiltroAsync(string filtro)
        {
            var lista = new List<Bodegas>();
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
            cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync()) lista.Add(MapearBodega(dr));
            return lista;
        }

        public async Task NuevaBodegaAsync(Bodegas oBodega)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
            cmd.Parameters.Add(new SqlParameter("@Nombre_Bodega", oBodega.Nombre_Bodega ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Id_Estado", oBodega.Id_Estado));
            cmd.Parameters.Add(new SqlParameter("@Id_Creador", oBodega.Id_Creador));
            cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oBodega.Id_Modificador));
            cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EditarBodegaAsync(Bodegas oBodega)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
            cmd.Parameters.Add(new SqlParameter("@Id_Bodega", oBodega.Id_Bodega));
            cmd.Parameters.Add(new SqlParameter("@Nombre_Bodega", oBodega.Nombre_Bodega ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Id_Estado", oBodega.Id_Estado));
            cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oBodega.Id_Modificador));
            cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task EliminarBodegaAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();
            using var cmd = new SqlCommand("sp_Tbl_Bodega", con) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
            cmd.Parameters.Add(new SqlParameter("@Id_Bodega", id));
            cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));
            cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
            cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });
            await cmd.ExecuteNonQueryAsync();
        }
    }
}