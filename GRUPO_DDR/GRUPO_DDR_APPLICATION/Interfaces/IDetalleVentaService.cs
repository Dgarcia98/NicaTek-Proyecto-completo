using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IDetalleVentaService
    {
        Task<IEnumerable<DetalleVentaDTO>> ListarDetalleVenta();
        Task<DetalleVentaDTO> ObtenerDetalleVentaPorId(int id);
        Task<IEnumerable<DetalleVentaDTO>> ListarDetalleVentaFiltro(string filtro);
        Task NuevoDetalleVenta(DetalleVentaDTO dto);
        Task EditarDetalleVenta(DetalleVentaDTO dto);
        Task EliminarDetalleVenta(int id, int idModificador);
    }
}