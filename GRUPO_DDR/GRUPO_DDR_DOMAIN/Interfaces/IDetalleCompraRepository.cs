using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IDetalleCompraRepository
    {
        Task<IEnumerable<DetalleCompra>> ListarDetalleCompraAsync();
        Task<IEnumerable<DetalleCompra>> FiltrarDetalleCompraAsync(string filtro);
        Task NuevoDetalleCompraAsync(DetalleCompra entidad);
        Task EditarDetalleCompraAsync(DetalleCompra entidad);
        Task EliminarDetalleCompraAsync(int id, int idModificador);
    }
}