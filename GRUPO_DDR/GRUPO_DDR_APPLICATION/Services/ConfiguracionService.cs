using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class ConfiguracionService : IConfiguracionService
    {
        private readonly IConfiguracionRepository _repository;

        public ConfiguracionService(IConfiguracionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ConfiguracionEmpresaDTO> ObtenerConfiguracion()
        {
            var p = await _repository.ObtenerConfiguracionAsync();
            if (p == null) return null;

            return new ConfiguracionEmpresaDTO
            {
                Id_Configuracion = p.Id_Configuracion,
                Nombre_Empresa = p.Nombre_Empresa,
                RUC_No = p.RUC_No,
                Direccion = p.Direccion,
                Telefono = p.Telefono,
                Correo_Empresa = p.Correo_Empresa,
                Ruta_Logo = p.Ruta_Logo,
                Moneda_Simbolo = p.Moneda_Simbolo,
                Fecha_Modificacion = p.Fecha_Modificacion
            };
        }

        public async Task NuevaConfiguracion(ConfiguracionEmpresaDTO dto)
        {
            var entidad = new ConfiguracionEmpresa
            {
                Nombre_Empresa = dto.Nombre_Empresa,
                RUC_No = dto.RUC_No,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Correo_Empresa = dto.Correo_Empresa,
                Ruta_Logo = dto.Ruta_Logo,
                Moneda_Simbolo = dto.Moneda_Simbolo,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.InsertarConfiguracionAsync(entidad);
        }

        public async Task EditarConfiguracion(ConfiguracionEmpresaDTO dto)
        {
            var entidad = new ConfiguracionEmpresa
            {
                Id_Configuracion = dto.Id_Configuracion,
                Nombre_Empresa = dto.Nombre_Empresa,
                RUC_No = dto.RUC_No,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                Correo_Empresa = dto.Correo_Empresa,
                Ruta_Logo = dto.Ruta_Logo,
                Moneda_Simbolo = dto.Moneda_Simbolo,
                Id_Modificador = dto.Id_Modificador
            };
            await _repository.ActualizarConfiguracionAsync(entidad);
        }
    }
}