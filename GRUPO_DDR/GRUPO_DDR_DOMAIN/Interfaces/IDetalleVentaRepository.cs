using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IDetalleVentaRepository
    {
        Task<IEnumerable<DetalleVenta>> ListarDetalleVentaAsync();
        Task<IEnumerable<DetalleVenta>> ListarDetalleVentaFiltroAsync(string filtro);
        Task NuevoDetalleVentaAsync(DetalleVenta detalle);
        Task EditarDetalleVentaAsync(DetalleVenta detalle);
        Task EliminarDetalleVentaAsync(int id, int idModificador);
    }
}