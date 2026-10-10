using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IDetalleCompraService
    {
        Task<IEnumerable<DetalleCompraDTO>> ListarDetalleCompra();
        Task<DetalleCompraDTO> ObtenerDetalleCompraPorId(int id);
        Task<IEnumerable<DetalleCompraDTO>> ListarPorFiltro(string Buscar);
        Task NuevoDetalleCompra(DetalleCompraDTO dto);
        Task EditarDetalleCompra(DetalleCompraDTO dto);
        Task EliminarDetalleCompra(int id, int idModificador);
    }
}