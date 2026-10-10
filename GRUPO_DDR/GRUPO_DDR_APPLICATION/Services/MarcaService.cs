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
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepository;

        public MarcaService(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        public async Task<IEnumerable<MarcaDTO>> ListarMarca()
        {
            var listar = await _marcaRepository.ListarMarcaAsync();

            return listar.Select(item => new MarcaDTO
            {
                Id_Marca = item.Id_Marca,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion ?? DateTime.MinValue,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador ?? 0
            });
        }

        public async Task<IEnumerable<MarcaDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<MarcaDTO>();

            var lista = await _marcaRepository.ListarMarcaFiltroAsync(Buscar);

            return lista.Select(item => new MarcaDTO
            {
                Id_Marca = item.Id_Marca,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion ?? DateTime.MinValue,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador ?? 0
            });
        }

        public async Task NuevoMarca(MarcaDTO oMarcaDTO)
        {
            var oMarca = new Marca
            {
                Nombre = oMarcaDTO.Nombre,
                Id_Estado = oMarcaDTO.Id_Estado,
                Id_Creador = oMarcaDTO.Id_Creador,
                Id_Modificador = oMarcaDTO.Id_Modificador
            };

            await _marcaRepository.NuevaMarcaAsync(oMarca);
        }

        public async Task EditarMarca(MarcaDTO oMarcaDTO)
        {
            var oMarca = new Marca
            {
                Id_Marca = oMarcaDTO.Id_Marca,
                Nombre = oMarcaDTO.Nombre,
                Id_Estado = oMarcaDTO.Id_Estado,
                Id_Modificador = oMarcaDTO.Id_Modificador
            };

            await _marcaRepository.EditarMarcaAsync(oMarca);
        }

        public async Task EliminarMarca(int id)
        {
            await _marcaRepository.EliminarMarcaAsync(id);
        }
    }
}