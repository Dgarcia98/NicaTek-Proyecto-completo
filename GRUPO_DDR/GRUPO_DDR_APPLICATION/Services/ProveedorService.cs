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
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;

        public ProveedorService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        public async Task<IEnumerable<ProveedorDTO>> ListarProveedor()
        {
            var listar = await _proveedorRepository.ListarProveedoresAsync();

            return listar.Select(p => new ProveedorDTO
            {
                Id_Proveedor = p.Id_Proveedor,
                Id_Persona = p.Id_Persona,
                Cedula = p.Cedula,
                Primer_Nombre = p.Primer_Nombre,
                Segundo_Nombre = p.Segundo_Nombre,
                Primer_Apellido = p.Primer_Apellido,
                Segundo_Apellido = p.Segundo_Apellido,
                Nombre_Completo = $"{p.Primer_Nombre} {p.Segundo_Nombre} {p.Primer_Apellido} {p.Segundo_Apellido}".Replace("  ", " ").Trim(),
                Id_Estado = p.Id_Estado,
                Estado = p.Estado,
                FechaRegistro = p.FechaRegistro,
                Fecha_Terminacion_Contrato = p.Fecha_Terminacion_Contrato
            });
        }

        public async Task<IEnumerable<ProveedorDTO>> ListarPorNombre(string Buscar)
        {
            if (string.IsNullOrWhiteSpace(Buscar))
                return Enumerable.Empty<ProveedorDTO>();

            var lista = await _proveedorRepository.ListarProveedoresNombreAsync(Buscar);

            return lista.Select(p => new ProveedorDTO
            {
                Id_Proveedor = p.Id_Proveedor,
                Id_Persona = p.Id_Persona,
                Cedula = p.Cedula,
                Primer_Nombre = p.Primer_Nombre,
                Segundo_Nombre = p.Segundo_Nombre,
                Primer_Apellido = p.Primer_Apellido,
                Segundo_Apellido = p.Segundo_Apellido,
                Nombre_Completo = $"{p.Primer_Nombre} {p.Segundo_Nombre} {p.Primer_Apellido} {p.Segundo_Apellido}".Replace("  ", " ").Trim(),
                Id_Estado = p.Id_Estado,
                Estado = p.Estado,
                FechaRegistro = p.FechaRegistro,
                Fecha_Terminacion_Contrato = p.Fecha_Terminacion_Contrato
            });
        }

        public async Task<ProveedorDTO> ObtenerProveedorPorId(int id)
        {
            var lista = await _proveedorRepository.ListarProveedoresNombreAsync(id.ToString());
            var p = lista.FirstOrDefault(item => item.Id_Proveedor == id);

            if (p == null) return null;

            return new ProveedorDTO
            {
                Id_Proveedor = p.Id_Proveedor,
                Id_Persona = p.Id_Persona,
                Cedula = p.Cedula,
                Primer_Nombre = p.Primer_Nombre,
                Segundo_Nombre = p.Segundo_Nombre,
                Primer_Apellido = p.Primer_Apellido,
                Segundo_Apellido = p.Segundo_Apellido,
                Nombre_Completo = $"{p.Primer_Nombre} {p.Segundo_Nombre} {p.Primer_Apellido} {p.Segundo_Apellido}".Replace("  ", " ").Trim(),
                Id_Estado = p.Id_Estado,
                Estado = p.Estado,
                FechaRegistro = p.FechaRegistro,
                Fecha_Terminacion_Contrato = p.Fecha_Terminacion_Contrato
            };
        }

        public async Task NuevoProveedor(ProveedorDTO dto)
        {
            var oProveedor = new Proveedores
            {
                Id_Persona = dto.Id_Persona, // ← fix: faltaba
                Cedula = dto.Cedula,
                Primer_Nombre = dto.Primer_Nombre,
                Segundo_Nombre = dto.Segundo_Nombre,
                Primer_Apellido = dto.Primer_Apellido,
                Segundo_Apellido = dto.Segundo_Apellido,
                Id_Estado = dto.Id_Estado,
                FechaRegistro = dto.FechaRegistro == DateTime.MinValue ? DateTime.Now : dto.FechaRegistro,
                Fecha_Terminacion_Contrato = dto.Fecha_Terminacion_Contrato,
                Id_Creador = 1,
                Id_Modificador = 1
            };

            await _proveedorRepository.NuevoProveedoresAsync(oProveedor);
        }

        public async Task EditarProveedor(ProveedorDTO dto)
        {
            var oProveedor = new Proveedores
            {
                Id_Proveedor = dto.Id_Proveedor,
                Id_Persona = dto.Id_Persona, // ← fix: faltaba
                Cedula = dto.Cedula,
                Primer_Nombre = dto.Primer_Nombre,
                Segundo_Nombre = dto.Segundo_Nombre,
                Primer_Apellido = dto.Primer_Apellido,
                Segundo_Apellido = dto.Segundo_Apellido,
                Id_Estado = dto.Id_Estado,
                FechaRegistro = dto.FechaRegistro,
                Fecha_Terminacion_Contrato = dto.Fecha_Terminacion_Contrato,
                Id_Modificador = 1
            };

            await _proveedorRepository.EditarProveedoresAsync(oProveedor);
        }

        public async Task EliminarProveedor(int id)
        {
            await _proveedorRepository.EliminarProveedoresAsync(id);
        }
    }
}