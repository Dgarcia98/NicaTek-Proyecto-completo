using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly ICatalogoRepository _repository;

        public CatalogoService(ICatalogoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CatalogoDTO>> ListarCatalogo()
        {
            var listar = await _repository.ListarCatalogosAsync();

            return listar.Select(item => new CatalogoDTO
            {
                Id_Catalogo = item.Id_Catalogo,
                Id_Tipo_Catalogo = item.Id_Tipo_Catalogo,
                Tipo_Catalogo_Nombre = item.Tipo_Catalogo_Nombre,
                Nombre_Catalogo = item.Nombre_Catalogo,
                Activo = item.Activo,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<IEnumerable<CatalogoDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<CatalogoDTO>();

            var lista = await _repository.ListarCatalogosFiltroAsync(Buscar);

            return lista.Select(item => new CatalogoDTO
            {
                Id_Catalogo = item.Id_Catalogo,
                Id_Tipo_Catalogo = item.Id_Tipo_Catalogo,
                Tipo_Catalogo_Nombre = item.Tipo_Catalogo_Nombre,
                Nombre_Catalogo = item.Nombre_Catalogo,
                Activo = item.Activo,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<CatalogoDTO> ObtenerCatalogoPorId(int id)
        {
            var lista = await _repository.ListarCatalogosFiltroAsync(id.ToString());
            var item = lista.FirstOrDefault();

            if (item == null) return null;

            return new CatalogoDTO
            {
                Id_Catalogo = item.Id_Catalogo,
                Id_Tipo_Catalogo = item.Id_Tipo_Catalogo,
                Tipo_Catalogo_Nombre = item.Tipo_Catalogo_Nombre,
                Nombre_Catalogo = item.Nombre_Catalogo,
                Activo = item.Activo,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task NuevoCatalogo(CatalogoDTO dto)
        {
            var oCatalogo = new Catalogo
            {
                Id_Tipo_Catalogo = dto.Id_Tipo_Catalogo,
                Nombre_Catalogo = dto.Nombre_Catalogo,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Creador,
                Activo = true
            };

            await _repository.NuevoCatalogoAsync(oCatalogo);
        }

        public async Task EditarCatalogo(CatalogoDTO dto)
        {
            var oCatalogo = new Catalogo
            {
                Id_Catalogo = dto.Id_Catalogo,
                Id_Tipo_Catalogo = dto.Id_Tipo_Catalogo,
                Nombre_Catalogo = dto.Nombre_Catalogo,
                Id_Modificador = dto.Id_Modificador,
                Activo = dto.Activo
            };

            await _repository.EditarCatalogoAsync(oCatalogo);
        }

        public async Task EliminarCatalogo(int id, int idModificador)
        {
            await _repository.EliminarCatalogoAsync(id, idModificador);
        }
    }
}