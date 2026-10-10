using GRUPO_DDR_DOMAIN.Entities;

public interface IConfiguracionRepository
{
    Task<ConfiguracionEmpresa> ObtenerConfiguracionAsync();
    Task InsertarConfiguracionAsync(ConfiguracionEmpresa config);
    Task ActualizarConfiguracionAsync(ConfiguracionEmpresa config);
}