using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IVentaRepository
    {
        Task<IEnumerable<Venta>> ListarVentasAsync();
        Task<IEnumerable<Venta>> ListarVentasFiltroAsync(string filtro);

        /// <summary>
        /// Registra cabecera + detalles + movimientos de inventario en una
        /// sola transaccion (sp_Venta_Registrar). O entra todo, o no entra
        /// nada. Devuelve el Id creado y el mensaje del procedimiento.
        /// </summary>
        Task<(int Id, string Mensaje)> RegistrarVentaAsync(
            int idCliente,
            int? idMetodoPago,
            DateTime? fecha,
            int idEstado,
            int idUsuario,
            IEnumerable<(int IdProducto, int? IdBodega, int Cantidad)> detalles);

        Task<int> NuevaVentaAsync(Venta venta);
        Task EditarVentaAsync(Venta venta);
        Task EliminarVentaAsync(int id, int idModificador);
    }
}