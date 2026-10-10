using GRUPO_DDR_APPLICATION.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface ICompraService
    {
        Task<IEnumerable<CompraDTO>> ListarCompra();
        Task<CompraDTO> ObtenerCompraPorId(int id);
        Task<IEnumerable<CompraDTO>> ListarPorFiltro(string Buscar);

        /// <summary>Registro atomico de la compra completa.</summary>
        Task<ResultadoRegistroDTO> RegistrarCompra(CompraRegistroDTO dto, int idUsuario);

        Task<int> NuevoCompra(CompraDTO dto);
        Task EditarCompra(CompraDTO dto);
        Task EliminarCompra(int id, int idModificador);
    }
}