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
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;

        public EmpleadoService(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }

        public async Task<IEnumerable<EmpleadoDTO>> ListarEmpleado()
        {
            var listar = await _empleadoRepository.ListarEmpleadosAsync();

            return listar.Select(item => new EmpleadoDTO
            {
                Id_Empleado = item.Id_Empleado,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Cedula = item.Cedula,
                Fecha_Contratacion = item.Fecha_Contratacion,
                Fecha_Despido = item.Fecha_Despido,
                Id_Cargo = item.Id_Cargo,
                Cargo = item.Cargo,
                Id_Estado = item.Id_Estado,
                Estado = item.Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<IEnumerable<EmpleadoDTO>> ListarPorFiltro(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<EmpleadoDTO>();

            var lista = await _empleadoRepository.FiltrarEmpleadosAsync(Buscar);

            return lista.Select(item => new EmpleadoDTO
            {
                Id_Empleado = item.Id_Empleado,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Cedula = item.Cedula,
                Fecha_Contratacion = item.Fecha_Contratacion,
                Fecha_Despido = item.Fecha_Despido,
                Id_Cargo = item.Id_Cargo,
                Cargo = item.Cargo,
                Id_Estado = item.Id_Estado,
                Estado = item.Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<EmpleadoDTO> ObtenerEmpleadoPorId(int id)
        {
            var lista = await _empleadoRepository.FiltrarEmpleadosAsync(id.ToString());
            var item = lista.FirstOrDefault();

            if (item == null) return null;

            return new EmpleadoDTO
            {
                Id_Empleado = item.Id_Empleado,
                Id_Persona = item.Id_Persona,
                Nombre_Completo = item.Nombre_Completo,
                Cedula = item.Cedula,
                Fecha_Contratacion = item.Fecha_Contratacion,
                Fecha_Despido = item.Fecha_Despido,
                Id_Cargo = item.Id_Cargo,
                Cargo = item.Cargo,
                Id_Estado = item.Id_Estado,
                Estado = item.Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task NuevoEmpleado(EmpleadoDTO dto)
        {
            var oEmpleado = new Empleado
            {
                Id_Persona = dto.Id_Persona,
                Fecha_Contratacion = dto.Fecha_Contratacion,
                Fecha_Despido = dto.Fecha_Despido,
                Id_Cargo = dto.Id_Cargo,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador,
                Id_Modificador = dto.Id_Modificador ?? dto.Id_Creador
            };

            await _empleadoRepository.NuevoEmpleadoAsync(oEmpleado);
        }

        public async Task EditarEmpleado(EmpleadoDTO dto)
        {
            var oEmpleado = new Empleado
            {
                Id_Empleado = dto.Id_Empleado,
                Id_Persona = dto.Id_Persona,
                Fecha_Contratacion = dto.Fecha_Contratacion,
                Fecha_Despido = dto.Fecha_Despido,
                Id_Cargo = dto.Id_Cargo,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador ?? 0
            };

            await _empleadoRepository.EditarEmpleadoAsync(oEmpleado);
        }

        public async Task EliminarEmpleado(int id)
        {
            await _empleadoRepository.EliminarEmpleadoAsync(id, 1);
        }
    }
}