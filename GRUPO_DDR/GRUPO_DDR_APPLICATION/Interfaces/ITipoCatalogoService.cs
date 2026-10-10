using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface ITipoCatalogoService
    {
        Task<IEnumerable<TipoCatalogoDTO>> ListarTipoCatalogo();

        Task<TipoCatalogoDTO> ObtenerTipoCatalogoPorId(int id);

        Task<IEnumerable<TipoCatalogoDTO>> ListarPorNombre(string Buscar);

        Task NuevoTipoCatalogo(TipoCatalogoDTO dto);

        Task EditarTipoCatalogo(TipoCatalogoDTO dto);

        Task EliminarTipoCatalogo(int id);
    }
}