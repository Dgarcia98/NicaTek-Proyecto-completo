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
    public class TipoCatalogoService : ITipoCatalogoService
    {
        private readonly ITipoCatalogoRepository _tipoCatalogoRepository;

        public TipoCatalogoService(ITipoCatalogoRepository tipoCatalogoRepository)
        {
            _tipoCatalogoRepository = tipoCatalogoRepository;
        }

        public async Task<IEnumerable<TipoCatalogoDTO>> ListarTipoCatalogo()
        {
            var listar = await _tipoCatalogoRepository.ListarTipoCatalogo();

            return listar.Select(item => new TipoCatalogoDTO
            {
                Id_Tipo_Catalogo = item.Id_Tipo_Catalogo,
                Tipo_Catalogo = item.Tipo_Catalogo,
                Activo = item.Estado,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador != null ? (int)item.Id_Modificador : 0,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion != null ? (DateTime)item.Fecha_Modificacion : DateTime.MinValue
            });
        }

        public async Task<IEnumerable<TipoCatalogoDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<TipoCatalogoDTO>();

            var lista = await _tipoCatalogoRepository.ListarPorNombre(Buscar);

            return lista.Select(item => new TipoCatalogoDTO
            {
                Id_Tipo_Catalogo = item.Id_Tipo_Catalogo,
                Tipo_Catalogo = item.Tipo_Catalogo,
                Activo = item.Estado,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador != null ? (int)item.Id_Modificador : 0,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion != null ? (DateTime)item.Fecha_Modificacion : DateTime.MinValue
            });
        }

        public async Task<TipoCatalogoDTO> ObtenerTipoCatalogoPorId(int id)
        {
            var item = await _tipoCatalogoRepository.ObtenerTipoCatalogoPorId(id);

            if (item == null) return null;

            return new TipoCatalogoDTO
            {
                Id_Tipo_Catalogo = item.Id_Tipo_Catalogo,
                Tipo_Catalogo = item.Tipo_Catalogo,
                Activo = item.Estado,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador != null ? (int)item.Id_Modificador : 0,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion != null ? (DateTime)item.Fecha_Modificacion : DateTime.MinValue
            };
        }

        public async Task NuevoTipoCatalogo(TipoCatalogoDTO dto)
        {
            var oEntidad = new TipoCatalogo
            {
                Tipo_Catalogo = dto.Tipo_Catalogo,
                Estado = dto.Activo,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador
            };

            await _tipoCatalogoRepository.NuevoTipoCatalogo(oEntidad);
        }

        public async Task EditarTipoCatalogo(TipoCatalogoDTO dto)
        {
            var oEntidad = new TipoCatalogo
            {
                Id_Tipo_Catalogo = dto.Id_Tipo_Catalogo,
                Tipo_Catalogo = dto.Tipo_Catalogo,
                Estado = dto.Activo,
                Id_Modificador = dto.Id_Modificador
            };

            await _tipoCatalogoRepository.EditarTipoCatalogo(oEntidad);
        }

        public async Task EliminarTipoCatalogo(int id)
        {
            int idModificadorDefecto = 1;
            await _tipoCatalogoRepository.EliminarTipoCatalogo(id, idModificadorDefecto);
        }
    }
}