using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class TelefonoService : ITelefonoService
    {
        private readonly ITelefonoRepository _telefonoRepository;

        public TelefonoService(ITelefonoRepository telefonoRepository)
        {
            _telefonoRepository = telefonoRepository;
        }

        public async Task<IEnumerable<TelefonoDTO>> ListarTelefono()
        {
            var telefonos = await _telefonoRepository.ListarTelefonosAsync();
            return telefonos.Select(t => new TelefonoDTO
            {
                Id_Telefono = t.Id_Telefono,
                Id_Persona = t.Id_Persona,
                Numero_Telefono = t.Numero_Telefono,
                Id_Tipo_Telefono = t.Id_Tipo_Telefono,
                Tipo_Telefono = t.Tipo_Telefono,
                Estado = t.Estado,
                Nombre_Estado = t.Nombre_Estado,
                Fecha_Creacion = t.Fecha_Creacion,
                Fecha_Modificacion = t.Fecha_Modificacion,
                Id_Creador = t.Id_Creador,
                Id_Modificador = t.Id_Modificador
            });
        }

        public async Task<TelefonoDTO> ObtenerTelefonoPorId(int id)
        {
            var telefonos = await _telefonoRepository.ListarTelefonosAsync();
            var t = telefonos.FirstOrDefault(x => x.Id_Telefono == id);

            if (t == null) return null;

            return new TelefonoDTO
            {
                Id_Telefono = t.Id_Telefono,
                Id_Persona = t.Id_Persona,
                Numero_Telefono = t.Numero_Telefono,
                Id_Tipo_Telefono = t.Id_Tipo_Telefono,
                Tipo_Telefono = t.Tipo_Telefono,
                Estado = t.Estado,
                Nombre_Estado = t.Nombre_Estado,
                Fecha_Creacion = t.Fecha_Creacion,
                Id_Creador = t.Id_Creador
            };
        }

        public async Task<IEnumerable<TelefonoDTO>> ListarPorNumero(string Buscar)
        {
            var telefonos = await _telefonoRepository.ListarTelefonosFiltroAsync(Buscar);
            return telefonos.Select(t => new TelefonoDTO
            {
                Id_Telefono = t.Id_Telefono,
                Id_Persona = t.Id_Persona,
                Numero_Telefono = t.Numero_Telefono,
                Id_Tipo_Telefono = t.Id_Tipo_Telefono,
                Tipo_Telefono = t.Tipo_Telefono,
                Estado = t.Estado,
                Nombre_Estado = t.Nombre_Estado,
                Fecha_Creacion = t.Fecha_Creacion
            });
        }

        public async Task NuevoTelefono(TelefonoDTO dto)
        {
            var oTelefono = new Telefonos
            {
                Id_Persona = dto.Id_Persona,
                Numero_Telefono = dto.Numero_Telefono,
                Id_Tipo_Telefono = dto.Id_Tipo_Telefono,
                Estado = dto.Estado,
                Id_Creador = dto.Id_Creador
            };

            await _telefonoRepository.NuevoTelefonoAsync(oTelefono);
        }

        public async Task EditarTelefono(TelefonoDTO dto)
        {
            var oTelefono = new Telefonos
            {
                Id_Telefono = dto.Id_Telefono,
                Id_Persona = dto.Id_Persona,
                Numero_Telefono = dto.Numero_Telefono,
                Id_Tipo_Telefono = dto.Id_Tipo_Telefono,
                Estado = dto.Estado,
                Id_Modificador = dto.Id_Modificador
            };

            await _telefonoRepository.EditarTelefonoAsync(oTelefono);
        }

        public async Task EliminarTelefono(int id)
        {
            await _telefonoRepository.EliminarTelefonoAsync(id, 0);
        }
    }
}