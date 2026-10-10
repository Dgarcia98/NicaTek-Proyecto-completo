using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ICompraRepository
    {
        Task<IEnumerable<Compra>> ListarCompraAsync();
        Task<IEnumerable<Compra>> FiltrarCompraAsync(string filtro);

        /// <summary>
        /// Registra cabecera + detalles + entradas de inventario en una
        /// sola transaccion (sp_Compra_Registrar).
        /// </summary>
        Task<(int Id, string Mensaje)> RegistrarCompraAsync(
            int idProveedor,
            string numeroFactura,
            DateTime? fechaCompra,
            int idEstado,
            int idUsuario,
            IEnumerable<(int IdProducto, int IdBodega, int Cantidad, decimal PrecioUnitario)> detalles);

        Task<int> NuevaCompraAsync(Compra entidad);
        Task EditarCompraAsync(Compra entidad);
        Task EliminarCompraAsync(int id, int idModificador);
    }
}