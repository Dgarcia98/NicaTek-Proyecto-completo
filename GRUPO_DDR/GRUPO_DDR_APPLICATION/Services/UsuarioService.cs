using GRUPO_DDR_APPLICATION.DTOs;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_DOMAIN.Entities;
using GRUPO_DDR_DOMAIN.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GRUPO_DDR_APPLICATION.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<UsuarioDTO> Login(LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Nombre_Usuario) ||
                string.IsNullOrWhiteSpace(dto?.Contrasena))
                return null;

            var usuario = await _usuarioRepository.Login(dto.Nombre_Usuario);

            // Verificar el hash incluso cuando el usuario no existe mantiene
            // constante el tiempo de respuesta: si no, la diferencia permite
            // enumerar que nombres de usuario son validos.
            const string HashSenuelo = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";
            bool esValida = BCrypt.Net.BCrypt.Verify(
                dto.Contrasena,
                usuario?.Contrasena ?? HashSenuelo);

            if (usuario == null || !esValida) return null;

            // Los permisos se resuelven en el servidor y viajan firmados en
            // el token. El frontend ya no puede concederselos escribiendo en
            // sessionStorage.
            var permisos = await _usuarioRepository.ObtenerPermisos(usuario.Id_Usuario);

            var token = GenerarToken(usuario, permisos);

            return new UsuarioDTO
            {
                Id_Usuario = usuario.Id_Usuario,
                Nombre_Usuario = usuario.Nombre_Usuario,
                Id_Rol = usuario.Id_Rol,
                Nombre_Rol = usuario.Nombre_Rol,
                Id_Persona = usuario.Id_Persona,
                Nombre_Persona = usuario.Nombre_Persona,
                Permisos = permisos.ToList(),
                Token = token
                // Contrasena queda deliberadamente fuera de la respuesta.
            };
        }

        private string GenerarToken(Usuario usuario, IEnumerable<string> permisos)
        {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Jwt:Key no esta configurada.");

            // UTF8 y no ASCII: ASCII descarta los bytes fuera de 0-127,
            // debilitando en silencio una clave con caracteres acentuados.
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id_Usuario.ToString()));
            claims.AddClaim(new Claim(ClaimTypes.Name, usuario.Nombre_Usuario));
            claims.AddClaim(new Claim(ClaimTypes.Role, usuario.Nombre_Rol ?? "Usuario"));

            foreach (var codigo in permisos.Distinct())
                claims.AddClaim(new Claim("permiso", codigo));

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
            );

            var minutos = int.TryParse(_configuration["Jwt:ExpiracionMinutos"], out var m) ? m : 480;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(minutos),
                SigningCredentials = credenciales,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenConfig);
        }

        public async Task NuevoUsuario(UsuarioDTO dto)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            var entidad = new Usuario
            {
                Nombre_Usuario = dto.Nombre_Usuario,
                Contrasena = passwordHash,
                Id_Persona = dto.Id_Persona,
                Id_Rol = dto.Id_Rol,
                Id_Estado = dto.Id_Estado,
                Id_Creador = dto.Id_Creador
            };

            await _usuarioRepository.NuevoUsuario(entidad);
        }

        public async Task EditarUsuario(UsuarioDTO dto)
        {
            string passwordToSend = null;

            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                passwordToSend = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            var entidad = new Usuario
            {
                Id_Usuario = dto.Id_Usuario,
                Nombre_Usuario = dto.Nombre_Usuario,
                Contrasena = passwordToSend,
                Id_Persona = dto.Id_Persona,
                Id_Rol = dto.Id_Rol,
                Id_Estado = dto.Id_Estado,
                Id_Modificador = dto.Id_Modificador
            };

            await _usuarioRepository.EditarUsuario(entidad);
        }

        public async Task<IEnumerable<UsuarioDTO>> ListarUsuarios()
        {
            var lista = await _usuarioRepository.ListarUsuarios();
            return lista.Select(item => new UsuarioDTO
            {
                Id_Usuario = item.Id_Usuario,
                Nombre_Usuario = item.Nombre_Usuario,
                Id_Persona = item.Id_Persona,
                Nombre_Persona = item.Nombre_Persona,
                Id_Rol = item.Id_Rol,
                Nombre_Rol = item.Nombre_Rol,
                Estado_Nombre = item.Estado_Nombre,
                Fecha_Ultimo_Acceso = item.Fecha_Ultimo_Acceso,
                Id_Estado = item.Id_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task<UsuarioDTO> ObtenerUsuarioPorId(int id)
        {
            var lista = await _usuarioRepository.ListarPorFiltro(id.ToString());
            var item = lista.FirstOrDefault(x => x.Id_Usuario == id);

            if (item == null) return null;

            return new UsuarioDTO
            {
                Id_Usuario = item.Id_Usuario,
                Nombre_Usuario = item.Nombre_Usuario,
                Id_Persona = item.Id_Persona,
                Nombre_Persona = item.Nombre_Persona,
                Id_Rol = item.Id_Rol,
                Nombre_Rol = item.Nombre_Rol,
                Fecha_Ultimo_Acceso = item.Fecha_Ultimo_Acceso,
                Id_Estado = item.Id_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            };
        }

        public async Task<IEnumerable<UsuarioDTO>> ListarPorFiltro(string filtro)
        {
            var lista = await _usuarioRepository.ListarPorFiltro(filtro);
            return lista.Select(item => new UsuarioDTO
            {
                Id_Usuario = item.Id_Usuario,
                Nombre_Usuario = item.Nombre_Usuario,
                Id_Persona = item.Id_Persona,
                Nombre_Persona = item.Nombre_Persona,
                Id_Rol = item.Id_Rol,
                Nombre_Rol = item.Nombre_Rol,
                Fecha_Ultimo_Acceso = item.Fecha_Ultimo_Acceso,
                Id_Estado = item.Id_Estado,
                Fecha_Creacion = item.Fecha_Creacion,
                Fecha_Modificacion = item.Fecha_Modificacion,
                Id_Creador = item.Id_Creador,
                Id_Modificador = item.Id_Modificador
            });
        }

        public async Task EliminarUsuario(int id, int idModificador)
        {
            await _usuarioRepository.EliminarUsuario(id, idModificador);
        }
    }
}