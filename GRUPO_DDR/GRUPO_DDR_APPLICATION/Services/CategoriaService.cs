using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoriaDTO>> ListarCategoria()
        {
            var listar = await _repository.ListarCategoriasAsync();

            return listar.Select(item => new CategoriaDTO
            {
                Id_Categoria = item.Id_Categoria,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<IEnumerable<CategoriaDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<CategoriaDTO>();

            var lista = await _repository.ListarCategoriasFiltroAsync(Buscar);

            return lista.Select(item => new CategoriaDTO
            {
                Id_Categoria = item.Id_Categoria,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<CategoriaDTO> ObtenerCategoriaPorId(int id)
        {
            var lista = await _repository.ListarCategoriasFiltroAsync(id.ToString());
            var item = lista.FirstOrDefault();

            if (item == null) return null;

            return new CategoriaDTO
            {
                Id_Categoria = item.Id_Categoria,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task NuevoCategoria(CategoriaDTO dto)
        {
            var oCategoria = new Categoria
            {
                Nombre = dto.Nombre,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Creador
            };

            await _repository.NuevoCategoriaAsync(oCategoria);
        }

        public async Task EditarCategoria(CategoriaDTO dto)
        {
            var oCategoria = new Categoria
            {
                Id_Categoria = dto.Id_Categoria,
                Nombre = dto.Nombre,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };

            await _repository.EditarCategoriaAsync(oCategoria);
        }

        public async Task EliminarCategoria(int id, int idModificador)
        {
            await _repository.EliminarCategoriaAsync(id, idModificador);
        }
    }
}