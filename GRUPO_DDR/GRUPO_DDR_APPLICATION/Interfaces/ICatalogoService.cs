using GRUPO_DDR_APPLICATION.DTOs;

public interface ICatalogoService
{
    Task<IEnumerable<CatalogoDTO>> ListarCatalogo();
    Task<CatalogoDTO> ObtenerCatalogoPorId(int id);
    Task<IEnumerable<CatalogoDTO>> ListarPorNombre(string Buscar);
    Task NuevoCatalogo(CatalogoDTO dto);
    Task EditarCatalogo(CatalogoDTO dto);
    Task EliminarCatalogo(int id, int idModificador);
}