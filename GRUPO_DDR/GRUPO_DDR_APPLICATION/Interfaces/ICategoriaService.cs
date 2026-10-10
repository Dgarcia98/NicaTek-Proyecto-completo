using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDTO>> ListarCategoria();
        Task<CategoriaDTO> ObtenerCategoriaPorId(int id);
        Task<IEnumerable<CategoriaDTO>> ListarPorNombre(string Buscar);
        Task NuevoCategoria(CategoriaDTO dto);
        Task EditarCategoria(CategoriaDTO dto);
        Task EliminarCategoria(int id, int idModificador);
    }
}