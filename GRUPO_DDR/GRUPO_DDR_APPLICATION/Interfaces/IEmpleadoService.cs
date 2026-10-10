using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IEmpleadoService
    {
        Task<IEnumerable<EmpleadoDTO>> ListarEmpleado();

        Task<EmpleadoDTO> ObtenerEmpleadoPorId(int id);

        Task<IEnumerable<EmpleadoDTO>> ListarPorFiltro(string Buscar);

        Task NuevoEmpleado(EmpleadoDTO dto);

        Task EditarEmpleado(EmpleadoDTO dto);

        Task EliminarEmpleado(int id);
    }
}