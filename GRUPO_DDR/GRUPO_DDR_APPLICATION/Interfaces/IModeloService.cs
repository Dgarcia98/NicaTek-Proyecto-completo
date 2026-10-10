using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IModeloService
    {
        Task<IEnumerable<ModeloDTO>> ListarModelo();
        Task<IEnumerable<ModeloDTO>> ListarPorNombre(string Buscar);
        Task NuevoModelo(ModeloDTO oModeloDTO);
        Task EditarModelo(ModeloDTO oModeloDTO);
        Task EliminarModelo(int id);
    }
}