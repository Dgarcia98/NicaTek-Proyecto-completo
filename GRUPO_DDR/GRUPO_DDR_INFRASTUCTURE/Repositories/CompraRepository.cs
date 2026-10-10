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
    public class CompraRepository : ICompraRepository
    {
        private readonly DbConection _conexion;

        public CompraRepository(DbConection conexion)
        {
            _conexion = conexion;
        }

        public async Task<IEnumerable<Compra>> ListarCompraAsync()
        {
            var olist = new List<Compra>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "LST"));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Compra
                        {
                            Id_Compra = Convert.ToInt32(dr["Id_Compra"]),
                            Id_Proveedor = Convert.ToInt32(dr["Id_Proveedor"]),
                            Proveedor = dr["Proveedor"].ToString(),
                            Numero_Factura_Proveedor = dr["Numero_Factura_Proveedor"].ToString(),
                            Fecha_Compra = Convert.ToDateTime(dr["Fecha_Compra"]),
                            Total_Compra = Convert.ToDecimal(dr["Total_Compra"]),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"].ToString(),
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

        public async Task<IEnumerable<Compra>> FiltrarCompraAsync(string filtro)
        {
            var olist = new List<Compra>();

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "FIL"));
                cmd.Parameters.Add(new SqlParameter("@Filtro", filtro));

                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Compra
                        {
                            Id_Compra = Convert.ToInt32(dr["Id_Compra"]),
                            Id_Proveedor = Convert.ToInt32(dr["Id_Proveedor"]),
                            Proveedor = dr["Proveedor"].ToString(),
                            Numero_Factura_Proveedor = dr["Numero_Factura_Proveedor"].ToString(),
                            Fecha_Compra = Convert.ToDateTime(dr["Fecha_Compra"]),
                            Total_Compra = Convert.ToDecimal(dr["Total_Compra"]),
                            Id_Estado = Convert.ToInt32(dr["Id_Estado"]),
                            Estado = dr["Estado"].ToString(),
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

        /// <summary>
        /// Envia todas las lineas de la compra en un table-valued parameter
        /// para que el procedimiento las procese en una unica transaccion.
        /// </summary>
        public async Task<(int Id, string Mensaje)> RegistrarCompraAsync(
            int idProveedor,
            string numeroFactura,
            DateTime? fechaCompra,
            int idEstado,
            int idUsuario,
            IEnumerable<(int IdProducto, int IdBodega, int Cantidad, decimal PrecioUnitario)> detalles)
        {
            // Debe coincidir con dbo.TVP_Detalle_Compra (003_registro_transaccional.sql)
            var tabla = new DataTable();
            tabla.Columns.Add("Id_Producto", typeof(int));
            tabla.Columns.Add("Id_Bodega", typeof(int));
            tabla.Columns.Add("Cantidad", typeof(int));
            tabla.Columns.Add("Precio_Unitario_Compra", typeof(decimal));

            foreach (var d in detalles)
                tabla.Rows.Add(d.IdProducto, d.IdBodega, d.Cantidad, d.PrecioUnitario);

            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using var cmd = new SqlCommand("sp_Compra_Registrar", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Id_Proveedor", SqlDbType.Int).Value = idProveedor;
            cmd.Parameters.Add("@Numero_Factura_Proveedor", SqlDbType.VarChar, 50).Value = numeroFactura;
            cmd.Parameters.Add("@Fecha_Compra", SqlDbType.Date).Value =
                fechaCompra.HasValue ? fechaCompra.Value.Date : (object)DBNull.Value;
            cmd.Parameters.Add("@Id_Estado", SqlDbType.Int).Value = idEstado;
            cmd.Parameters.Add("@Id_Usuario", SqlDbType.Int).Value = idUsuario;

            var tvp = cmd.Parameters.AddWithValue("@Detalles", tabla);
            tvp.SqlDbType = SqlDbType.Structured;
            tvp.TypeName = "dbo.TVP_Detalle_Compra";

            var oNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var oMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(oNum);
            cmd.Parameters.Add(oMsg);

            await cmd.ExecuteNonQueryAsync();

            int id = oNum.Value != DBNull.Value ? Convert.ToInt32(oNum.Value) : -1;
            string msg = oMsg.Value != DBNull.Value
                ? oMsg.Value.ToString()
                : "Respuesta vacia del procedimiento.";

            return (id, msg);
        }

        public async Task<int> NuevaCompraAsync(Compra entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "INS"));
                cmd.Parameters.Add(new SqlParameter("@Id_Proveedor", entidad.Id_Proveedor));
                cmd.Parameters.Add(new SqlParameter("@Numero_Factura_Proveedor", entidad.Numero_Factura_Proveedor ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Compra", entidad.Fecha_Compra));  // ✅ FIX 2: parámetro agregado
                cmd.Parameters.Add(new SqlParameter("@Total_Compra", entidad.Total_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Creador", entidad.Id_Creador));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                SqlParameter pMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
                SqlParameter pNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(pMsg);
                cmd.Parameters.Add(pNum);

                await cmd.ExecuteNonQueryAsync();

                int idGenerado = pNum.Value != DBNull.Value ? Convert.ToInt32(pNum.Value) : 0;

                if (idGenerado <= 0)
                    throw new Exception(pMsg.Value?.ToString() ?? "No se pudo registrar la compra.");

                return idGenerado;
            }
        }

        public async Task EditarCompraAsync(Compra entidad)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "UPD"));
                cmd.Parameters.Add(new SqlParameter("@Id_Compra", entidad.Id_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Proveedor", entidad.Id_Proveedor));
                cmd.Parameters.Add(new SqlParameter("@Numero_Factura_Proveedor", entidad.Numero_Factura_Proveedor ?? (object)DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Fecha_Compra", entidad.Fecha_Compra));
                cmd.Parameters.Add(new SqlParameter("@Total_Compra", entidad.Total_Compra));
                cmd.Parameters.Add(new SqlParameter("@Id_Estado", entidad.Id_Estado));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", entidad.Id_Modificador));

                cmd.Parameters.Add(new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output });

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarCompraAsync(int id, int idModificador)
        {
            using var con = _conexion.CreateConection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("sp_Tbl_Compra", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Mode", "DEL"));
                cmd.Parameters.Add(new SqlParameter("@Id_Compra", id));
                cmd.Parameters.Add(new SqlParameter("@Id_Modificador", idModificador));

                SqlParameter pMsg = new SqlParameter("@O_Msg", SqlDbType.VarChar, 255) { Direction = ParameterDirection.Output };
                SqlParameter pNum = new SqlParameter("@O_Num", SqlDbType.Int) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(pMsg);
                cmd.Parameters.Add(pNum);

                await cmd.ExecuteNonQueryAsync();

                // ✅ FIX 3: leer respuesta del SP y lanzar excepción si falló
                int resultado = pNum.Value != DBNull.Value ? Convert.ToInt32(pNum.Value) : 0;

                if (resultado == -1)
                    throw new Exception(pMsg.Value?.ToString() ?? "Error al anular la compra.");
            }
        }
    }
}