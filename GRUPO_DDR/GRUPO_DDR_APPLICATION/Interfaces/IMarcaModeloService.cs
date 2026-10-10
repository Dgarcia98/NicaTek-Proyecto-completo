using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IMarcaModeloService
    {
        Task<IEnumerable<MarcaModeloDTO>> ListarMarcaModelo();
        Task<IEnumerable<MarcaModeloDTO>> ListarPorNombre(string Buscar);
        Task NuevoMarcaModelo(MarcaModeloDTO dto);
        Task EditarMarcaModelo(MarcaModeloDTO dto);
        Task EliminarMarcaModelo(int id);
    }
}