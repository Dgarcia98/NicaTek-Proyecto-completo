using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaDTO>> ListarVentas();
        Task<VentaDTO> ObtenerVentaPorId(int id);
        Task<IEnumerable<VentaDTO>> ListarVentasFiltro(string Buscar);

        /// <summary>
        /// Registro atomico de la venta completa. El precio, el total y el
        /// usuario creador los determina el servidor.
        /// </summary>
        Task<ResultadoRegistroDTO> RegistrarVenta(VentaRegistroDTO dto, int idUsuario);

        Task<int> NuevaVenta(VentaDTO dto);
        Task EditarVenta(VentaDTO dto);
        Task EliminarVenta(int id, int idModificador);
    }
}