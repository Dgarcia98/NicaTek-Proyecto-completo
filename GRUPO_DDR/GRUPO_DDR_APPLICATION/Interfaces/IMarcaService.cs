using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IMarcaService
    {
        Task<IEnumerable<MarcaDTO>> ListarMarca();
        Task<IEnumerable<MarcaDTO>> ListarPorNombre(string Buscar);
        Task NuevoMarca(MarcaDTO oMarcaDTO);
        Task EditarMarca(MarcaDTO oMarcaDTO);
        Task EliminarMarca(int id);
    }
}