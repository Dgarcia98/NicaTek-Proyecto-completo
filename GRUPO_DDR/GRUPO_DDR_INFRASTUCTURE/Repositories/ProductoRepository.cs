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
    public class ProductoRepository : IProductoRepository
    {
        private readonly DbConection _conexion;

        public ProductoRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Productos>> ListarProductoAsync()
        {
            var olist = new List<Productos>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Producto", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Productos
                        {
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Precio_Compra = dr["Precio_Compra"] != DBNull.Value ? Convert.ToDecimal(dr["Precio_Compra"]) : 0,
                            Id_Marca_Modelo = Convert.ToInt32(dr["Id_Marca_Modelo"]),
                            Id_Proveedor = dr["Id_Proveedor"] != DBNull.Value ? (int?)Convert.ToInt32(dr["Id_Proveedor"]) : null,
                            Id_Categoria = dr["Id_Categoria"] != DBNull.Value ? (int?)Convert.ToInt32(dr["Id_Categoria"]) : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0,

                            Nombre_Marca = dr["Nombre_Marca"] != DBNull.Value ? dr["Nombre_Marca"].ToString() : "",
                            Nombre_Modelo = dr["Nombre_Modelo"] != DBNull.Value ? dr["Nombre_Modelo"].ToString() : "",
                            Nombre_Proveedor = dr["Proveedor"] != DBNull.Value ? dr["Proveedor"].ToString() : ""
                        });
                    }
                }
            }
            return olist;
        }

        public async Task<IEnumerable<Productos>> ListarProductosFiltroAsync(string filtro)
        {
            var olist = new List<Productos>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Producto", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Productos
                        {
                            Id_Producto = Convert.ToInt32(dr["Id_Producto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Precio_Compra = dr["Precio_Compra"] != DBNull.Value ? Convert.ToDecimal(dr["Precio_Compra"]) : 0,
                            Id_Marca_Modelo = Convert.ToInt32(dr["Id_Marca_Modelo"]),
                            Id_Proveedor = dr["Id_Proveedor"] != DBNull.Value ? (int?)Convert.ToInt32(dr["Id_Proveedor"]) : null,
                            Id_Categoria = dr["Id_Categoria"] != DBNull.Value ? (int?)Convert.ToInt32(dr["Id_Categoria"]) : null,
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Fecha_Creacion = dr["Fecha_Creacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Creacion"]) : DateTime.MinValue,
                            Fecha_Modificacion = dr["Fecha_Modificacion"] != DBNull.Value ? Convert.ToDateTime(dr["Fecha_Modificacion"]) : DateTime.MinValue,
                            Id_Creador = dr["Id_Creador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Creador"]) : 0,
                            Id_Modificador = dr["Id_Modificador"] != DBNull.Value ? Convert.ToInt32(dr["Id_Modificador"]) : 0,

                            Nombre_Marca = dr["Nombre_Marca"] != DBNull.Value ? dr["Nombre_Marca"].ToString() : "",
                            Nombre_Modelo = dr["Nombre_Modelo"] != DBNull.Value ? dr["Nombre_Modelo"].ToString() : "",
                            Nombre_Proveedor = dr["Proveedor"] != DBNull.Value ? dr["Proveedor"].ToString() : ""
                        });
                    }
                }
            }
            return olist;
        }

        public async Task NuevoProductoAsync(Productos oProducto)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Producto", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Codigo", oProducto.Codigo));
                cmd.Parameters.Add(new SqlParameter("@Nombre", oProducto.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", oProducto.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Precio", oProducto.Precio));
                cmd.Parameters.Add(new SqlParameter("@Precio_Compra", oProducto.Precio_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Marca_Modelo", oProducto.Id_Marca_Modelo));
                cmd.Parameters.Add(new SqlParameter("@Id_Proveedor", (object)oProducto.Id_Proveedor ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Categoria", (object)oProducto.Id_Categoria ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oProducto.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", oProducto.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oProducto.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarProductoAsync(Productos oProducto)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Producto", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Producto", oProducto.Id_Producto));
                cmd.Parameters.Add(new SqlParameter("@Codigo", oProducto.Codigo));
                cmd.Parameters.Add(new SqlParameter("@Nombre", oProducto.Nombre));
                cmd.Parameters.Add(new SqlParameter("@Descripcion", oProducto.Descripcion));
                cmd.Parameters.Add(new SqlParameter("@Precio", oProducto.Precio));
                cmd.Parameters.Add(new SqlParameter("@Precio_Compra", oProducto.Precio_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Marca_Modelo", oProducto.Id_Marca_Modelo));
                cmd.Parameters.Add(new SqlParameter("@Id_Proveedor", (object)oProducto.Id_Proveedor ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Categoria", (object)oProducto.Id_Categoria ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", oProducto.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", oProducto.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarProductoAsync(int id)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Producto", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Producto", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", 1));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}