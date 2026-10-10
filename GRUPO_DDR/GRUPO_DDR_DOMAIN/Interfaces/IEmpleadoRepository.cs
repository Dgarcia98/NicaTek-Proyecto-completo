using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<IEnumerable<Empleado>> ListarEmpleadosAsync();
        Task<IEnumerable<Empleado>> FiltrarEmpleadosAsync(string filtro);

        Task NuevoEmpleadoAsync(Empleado entidad);

        Task EditarEmpleadoAsync(Empleado entidad);
        Task EliminarEmpleadoAsync(int idEmpleado, int idModificador);
    }
}