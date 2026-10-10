using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IMetodoPagoRepository
    {
        Task<IEnumerable<MetodoPago>> ListarMetodosPagoAsync();
        Task<IEnumerable<MetodoPago>> ListarMetodosPagoFiltroAsync(string filtro);
        Task NuevoMetodoPagoAsync(MetodoPago metodo);
        Task EditarMetodoPagoAsync(MetodoPago metodo);
        Task EliminarMetodoPagoAsync(int id, int idModificador);
    }
}