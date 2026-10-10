using GRUPO_DDR_APPLICATION.DTOs;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Interfaces
{
    public interface IConfiguracionService
    {
        Task<ConfiguracionEmpresaDTO> ObtenerConfiguracion();
        Task NuevaConfiguracion(ConfiguracionEmpresaDTO dto);
        Task EditarConfiguracion(ConfiguracionEmpresaDTO dto);
    }
}