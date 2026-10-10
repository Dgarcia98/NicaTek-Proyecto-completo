using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class DatosPersonalesService : IDatosPersonalesService
    {
        private readonly IDatosPersonalesRepository _repository;

        public DatosPersonalesService(IDatosPersonalesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DatosPersonalesDTO>> ListarDatosPersonales()
        {
            var listar = await _repository.ObtenerTodosAsync();

            return listar.Select(x => new DatosPersonalesDTO
            {
                Id_Persona = x.Id_Persona,
                Primer_Nombre = x.Primer_Nombre,
                Segundo_Nombre = x.Segundo_Nombre,
                Primer_Apellido = x.Primer_Apellido,
                Segundo_Apellido = x.Segundo_Apellido,
                Id_Tipo_DNI = x.Id_Tipo_DNI,
                DNI = x.DNI,
                Id_Genero = x.Id_Genero,
                Id_Nacionalidad = x.Id_Nacionalidad,
                Id_Creador = x.Id_Creador,
                Id_Modificador = x.Id_Modificador,
                Estado = x.Estado
            });
        }

        public async Task<IEnumerable<DatosPersonalesDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<DatosPersonalesDTO>();

            var lista = await _repository.ObtenerPorFiltroAsync(Buscar);

            return lista.Select(x => new DatosPersonalesDTO
            {
                Id_Persona = x.Id_Persona,
                Primer_Nombre = x.Primer_Nombre,
                Segundo_Nombre = x.Segundo_Nombre,
                Primer_Apellido = x.Primer_Apellido,
                Segundo_Apellido = x.Segundo_Apellido,
                Id_Tipo_DNI = x.Id_Tipo_DNI,
                DNI = x.DNI,
                Id_Genero = x.Id_Genero,
                Id_Nacionalidad = x.Id_Nacionalidad,
                Id_Creador = x.Id_Creador,
                Id_Modificador = x.Id_Modificador,
                Estado = x.Estado
            });
        }

        public async Task<DatosPersonalesDTO> ObtenerDatosPersonalesPorId(int id)
        {
            var lista = await _repository.ObtenerPorFiltroAsync(id.ToString());
            var x = lista.FirstOrDefault(p => p.Id_Persona == id); // ← filtrar por ID exacto

            if (x == null) return null;

            return new DatosPersonalesDTO
            {
                Id_Persona = x.Id_Persona,
                Primer_Nombre = x.Primer_Nombre,
                Segundo_Nombre = x.Segundo_Nombre,
                Primer_Apellido = x.Primer_Apellido,
                Segundo_Apellido = x.Segundo_Apellido,
                Id_Tipo_DNI = x.Id_Tipo_DNI,
                DNI = x.DNI,
                Id_Genero = x.Id_Genero,
                Id_Nacionalidad = x.Id_Nacionalidad,
                Id_Creador = x.Id_Creador,
                Id_Modificador = x.Id_Modificador,
                Estado = x.Estado
            };
        }

        public async Task NuevoDatosPersonales(DatosPersonalesDTO dto)
        {
            var oDatosPersonales = new DatosPersonales
            {
                Primer_Nombre = dto.Primer_Nombre,
                Segundo_Nombre = dto.Segundo_Nombre,
                Primer_Apellido = dto.Primer_Apellido,
                Segundo_Apellido = dto.Segundo_Apellido,
                Id_Tipo_DNI = dto.Id_Tipo_DNI,
                DNI = dto.DNI,
                Id_Genero = dto.Id_Genero,
                Id_Nacionalidad = dto.Id_Nacionalidad,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador,
                Estado = dto.Estado
            };

            await _repository.InsertarAsync(oDatosPersonales);
        }

        public async Task EditarDatosPersonales(DatosPersonalesDTO dto)
        {
            var oDatosPersonales = new DatosPersonales
            {
                Id_Persona = dto.Id_Persona,
                Primer_Nombre = dto.Primer_Nombre,
                Segundo_Nombre = dto.Segundo_Nombre,
                Primer_Apellido = dto.Primer_Apellido,
                Segundo_Apellido = dto.Segundo_Apellido,
                Id_Tipo_DNI = dto.Id_Tipo_DNI,
                DNI = dto.DNI,
                Id_Genero = dto.Id_Genero,
                Id_Nacionalidad = dto.Id_Nacionalidad,
                Id_Modificador = dto.Id_Modificador,
                Estado = dto.Estado
            };

            await _repository.ActualizarAsync(oDatosPersonales);
        }

        public async Task EliminarDatosPersonales(int id)
        {
            await _repository.EliminarAsync(id);
        }
    }
}