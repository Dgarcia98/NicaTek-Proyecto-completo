using GRUPO_DDR_APPLICATION.DTOs;

public interface IDatosPersonalesService
{
    Task<IEnumerable<DatosPersonalesDTO>> ListarDatosPersonales();
    Task<DatosPersonalesDTO> ObtenerDatosPersonalesPorId(int id);
    Task<IEnumerable<DatosPersonalesDTO>> ListarPorNombre(string Buscar);
    Task NuevoDatosPersonales(DatosPersonalesDTO dto);
    Task EditarDatosPersonales(DatosPersonalesDTO dto);
    Task EliminarDatosPersonales(int id);
}