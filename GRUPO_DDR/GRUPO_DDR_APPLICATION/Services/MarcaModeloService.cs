using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class MarcaModeloService : IMarcaModeloService
    {
        private readonly IMarcaModeloRepository _repo;

        public MarcaModeloService(IMarcaModeloRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MarcaModeloDTO>> ListarMarcaModelo()
        {
            var listar = await _repo.ListarMarcaModeloAsync();

            return listar.Select(item => new MarcaModeloDTO
            {
                Id_Marca_Modelo = item.Id_Marca_Modelo,
                Id_Marca = item.Id_Marca,
                Nombre_Marca = item.Nombre_Marca,
                Id_Modelo = item.Id_Modelo,
                Nombre_Modelo = item.Nombre_Modelo,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador ?? 0,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion ?? DateTime.MinValue
            });
        }

        public async Task<IEnumerable<MarcaModeloDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<MarcaModeloDTO>();

            var lista = await _repo.ListarMarcaModeloFiltroAsync(Buscar);

            return lista.Select(item => new MarcaModeloDTO
            {
                Id_Marca_Modelo = item.Id_Marca_Modelo,
                Id_Marca = item.Id_Marca,
                Nombre_Marca = item.Nombre_Marca,
                Id_Modelo = item.Id_Modelo,
                Nombre_Modelo = item.Nombre_Modelo,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador ?? 0,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion ?? DateTime.MinValue
            });
        }

        public async Task NuevoMarcaModelo(MarcaModeloDTO dto)
        {
            var oMarcaModelo = new MarcaModelo
            {
                Id_Marca = dto.Id_Marca,
                Id_Modelo = dto.Id_Modelo,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };

            await _repo.NuevaMarcaModeloAsync(oMarcaModelo);
        }

        public async Task EditarMarcaModelo(MarcaModeloDTO dto)
        {
            var oMarcaModelo = new MarcaModelo
            {
                Id_Marca_Modelo = dto.Id_Marca_Modelo,
                Id_Marca = dto.Id_Marca,
                Id_Modelo = dto.Id_Modelo,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };

            await _repo.EditarMarcaModeloAsync(oMarcaModelo);
        }

        public async Task EliminarMarcaModelo(int id)
        {
            await _repo.EliminarMarcaModeloAsync(id);
        }
    }
}