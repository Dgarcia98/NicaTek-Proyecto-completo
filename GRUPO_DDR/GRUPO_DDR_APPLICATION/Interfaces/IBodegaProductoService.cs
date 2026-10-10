using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IBodegaProductoService
    {
        Task<IEnumerable<BodegaProductoDTO>> ListarBodegaProducto();
        Task<IEnumerable<BodegaProductoDTO>> FiltrarBodegaProducto(string filtro);
        Task<IEnumerable<BodegaProductoDTO>> ListarPorBodega(int idBodega);
        Task NuevoBodegaProducto(BodegaProductoDTO dto);
        Task EditarBodegaProducto(BodegaProductoDTO dto);
        Task EliminarBodegaProducto(int id, int idModificador);
    }
}