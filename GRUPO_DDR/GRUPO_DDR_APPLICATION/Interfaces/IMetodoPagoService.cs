using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IMetodoPagoService
    {
        Task<IEnumerable<MetodoPagoDTO>> ListarMetodoPago();
        Task<MetodoPagoDTO> ObtenerMetodoPagoPorId(int id);
        Task<IEnumerable<MetodoPagoDTO>> ListarPorNombre(string Buscar);
        Task NuevoMetodoPago(MetodoPagoDTO dto);
        Task EditarMetodoPago(MetodoPagoDTO dto);
        Task EliminarMetodoPago(int id, int idModificador);
    }
}