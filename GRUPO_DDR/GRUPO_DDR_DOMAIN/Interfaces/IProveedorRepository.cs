using GRUPO_DDR_DOMAIN.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_DOMAIN.Interfaces
{
    public interface IProveedorRepository
    {
        Task<IEnumerable<Proveedores>> ListarProveedoresAsync();
        Task<IEnumerable<Proveedores>> ListarProveedoresNombreAsync(string filtro);

        Task NuevoProveedoresAsync(Proveedores oProveedor);

        Task EditarProveedoresAsync(Proveedores oProveedor);

        Task EliminarProveedoresAsync(int id);
    }
}