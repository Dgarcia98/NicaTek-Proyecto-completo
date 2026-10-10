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
    public class ModeloService : IModeloService
    {
        private readonly IModeloRepository _modeloRepository;

        public ModeloService(IModeloRepository modeloRepository)
        {
            _modeloRepository = modeloRepository;
        }

        public async Task<IEnumerable<ModeloDTO>> ListarModelo()
        {
            var listar = await _modeloRepository.ListarModeloAsync();

            return listar.Select(item => new ModeloDTO
            {
                Id_Modelo = item.Id_Modelo,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion ?? DateTime.MinValue,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador ?? 0
            });
        }

        public async Task<IEnumerable<ModeloDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<ModeloDTO>();

            var lista = await _modeloRepository.ListarModeloFiltroAsync(Buscar);

            return lista.Select(item => new ModeloDTO
            {
                Id_Modelo = item.Id_Modelo,
                Nombre = item.Nombre,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion ?? DateTime.MinValue,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador ?? 0
            });
        }

        public async Task NuevoModelo(ModeloDTO oModeloDTO)
        {
            var oModelo = new Modelo
            {
                Nombre = oModeloDTO.Nombre,
                Id_Estado = oModeloDTO.Id_Estado,
                Id_Creador = oModeloDTO.Id_Creador,
                Id_Modificador = oModeloDTO.Id_Modificador
            };

            await _modeloRepository.NuevaModeloAsync(oModelo);
        }

        public async Task EditarModelo(ModeloDTO oModeloDTO)
        {
            var oModelo = new Modelo
            {
                Id_Modelo = oModeloDTO.Id_Modelo,
                Nombre = oModeloDTO.Nombre,
                Id_Estado = oModeloDTO.Id_Estado,
                Id_Modificador = oModeloDTO.Id_Modificador
            };

            await _modeloRepository.EditarModeloAsync(oModelo);
        }

        public async Task EliminarModelo(int id)
        {
            await _modeloRepository.EliminarModeloAsync(id);
        }
    }
}