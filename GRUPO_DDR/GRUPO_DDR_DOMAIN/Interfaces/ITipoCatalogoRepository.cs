using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface ITipoCatalogoRepository
    {
        Task<IEnumerable<TipoCatalogo>> ListarTipoCatalogo();
        Task<TipoCatalogo> ObtenerTipoCatalogoPorId(int id);
        Task<IEnumerable<TipoCatalogo>> ListarPorNombre(string nombre);
        Task NuevoTipoCatalogo(TipoCatalogo entidad);
        Task EditarTipoCatalogo(TipoCatalogo entidad);
        Task EliminarTipoCatalogo(int id, int idModificador);
    }
}