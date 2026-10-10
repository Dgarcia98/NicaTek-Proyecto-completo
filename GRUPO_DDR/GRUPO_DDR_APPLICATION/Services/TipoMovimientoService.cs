using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class TipoMovimientoService : ITipoMovimientoService
    {
        private readonly ITipoMovimientoRepository _tipoMovimientoRepository;

        public TipoMovimientoService(ITipoMovimientoRepository tipoMovimientoRepository)
        {
            _tipoMovimientoRepository = tipoMovimientoRepository;
        }

        public async Task<IEnumerable<TipoMovimientoDTO>> ListarTipoMovimiento()
        {
            var lista = await _tipoMovimientoRepository.ListarTipoMovimiento();
            return lista.Select(item => new TipoMovimientoDTO
            {
                Id_Tipo_Movimiento = item.Id_Tipo_Movimiento,
                Descripcion = item.Descripcion,
                Operacion = item.Operacion,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Activo = item.Activo,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion
            });
        }

        public async Task<TipoMovimientoDTO> ObtenerTipoMovimientoPorId(int id)
        {
            var lista = await _tipoMovimientoRepository.ListarTipoMovimiento();
            var item = lista.FirstOrDefault(x => x.Id_Tipo_Movimiento == id);

            if (item == null) return null;

            return new TipoMovimientoDTO
            {
                Id_Tipo_Movimiento = item.Id_Tipo_Movimiento,
                Descripcion = item.Descripcion,
                Operacion = item.Operacion,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Activo = item.Activo,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion
            };
        }

        public async Task<IEnumerable<TipoMovimientoDTO>> ListarPorNombre(string Buscar)
        {
            var lista = await _tipoMovimientoRepository.ListarPorNombre(Buscar);
            return lista.Select(item => new TipoMovimientoDTO
            {
                Id_Tipo_Movimiento = item.Id_Tipo_Movimiento,
                Descripcion = item.Descripcion,
                Operacion = item.Operacion,
                Id_Estado = item.Id_Estado,
                Nombre_Estado = item.Nombre_Estado,
                Activo = item.Activo,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion
            });
        }

        public async Task NuevoTipoMovimiento(TipoMovimientoDTO dto)
        {
            var entidad = new TipoMovimiento
            {
                Descripcion = dto.Descripcion,
                Operacion = dto.Operacion,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador
            };
            await _tipoMovimientoRepository.NuevoTipoMovimiento(entidad);
        }

        public async Task EditarTipoMovimiento(TipoMovimientoDTO dto)
        {
            var entidad = new TipoMovimiento
            {
                Id_Tipo_Movimiento = dto.Id_Tipo_Movimiento,
                Descripcion = dto.Descripcion,
                Operacion = dto.Operacion,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };
            await _tipoMovimientoRepository.EditarTipoMovimiento(entidad);
        }

        public async Task EliminarTipoMovimiento(int id)
        {
            await _tipoMovimientoRepository.EliminarTipoMovimiento(id, 1);
        }
    }
}