using System.Text;
using GRUPO_DDR_API.Authorization;
using GRUPO_DDR_API.Middleware;
using GRUPO_DDR_API.Services;
using GRUPO_DDR_APPLICATION.Interfaces;
using GRUPO_DDR_APPLICATION.Services;
using GRUPO_DDR_DOMAIN.Interfaces;
using GRUPO_DDR_INFRASTRUCTURE.Data;
using GRUPO_DDR_INFRASTRUCTURE.Repositories;
using GRUPO_DDR_INFRASTUCTURE.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// CONFIGURACION OBLIGATORIA
// ---------------------------------------------------------------------
// La app se niega a arrancar sin cadena de conexion o sin clave JWT.
// Es deliberado: antes ambas estaban escritas en appsettings.json y se
// versionaban en el repositorio. Fallar en el arranque es mucho mejor
// que desplegar produccion firmando tokens con una clave de ejemplo.
//
// Desarrollo -> dotnet user-secrets
// Produccion -> Azure App Service > Configuration > Application settings
//               ConnectionStrings__CadenaSQL  y  Jwt__Key
// =====================================================================
// La cadena de conexion local vive en appsettings.json: usa autenticacion
// integrada de Windows, no lleva credenciales.
var connectionString = builder.Configuration.GetConnectionString("CadenaSQL");

// La clave de firma SI es un secreto y no puede ir a git. Orden de busqueda:
//   1. Configuracion (user-secrets, variable de entorno Jwt__Key, appsettings)
//   2. Archivo local .jwt-dev-key junto al proyecto (ignorado por git)
//   3. Se genera una nueva y se guarda en ese archivo
//
// El paso 3 es lo que hace que la API arranque sin configurar nada a mano.
// Antes esto dependia de user-secrets, que SOLO se cargan cuando el entorno
// es Development: al arrancar sin ASPNETCORE_ENVIRONMENT (por defecto es
// Production) la clave no aparecia y la API se negaba a iniciar.
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    var rutaClave = Path.Combine(builder.Environment.ContentRootPath, ".jwt-dev-key");

    if (File.Exists(rutaClave))
    {
        jwtKey = File.ReadAllText(rutaClave).Trim();
    }

    if (string.IsNullOrWhiteSpace(jwtKey))
    {
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(64);
        jwtKey = Convert.ToBase64String(bytes);

        try
        {
            File.WriteAllText(rutaClave, jwtKey);
            Console.WriteLine($"[NicaTek] Clave JWT generada y guardada en {rutaClave}");
            Console.WriteLine("[NicaTek] Ese archivo esta en .gitignore. Borralo para regenerarla (cierra las sesiones abiertas).");
        }
        catch (Exception ex)
        {
            // Si no se puede escribir, la API igual arranca: la clave vive
            // en memoria. El costo es que al reiniciar cambia y se cierran
            // las sesiones, por eso se avisa.
            Console.WriteLine($"[NicaTek] Aviso: no se pudo guardar la clave JWT ({ex.Message}).");
            Console.WriteLine("[NicaTek] Se usara una clave temporal: las sesiones se cerraran al reiniciar la API.");
        }
    }
}

var faltantes = new List<string>();
if (string.IsNullOrWhiteSpace(connectionString))
    faltantes.Add("ConnectionStrings:CadenaSQL");
if (string.IsNullOrWhiteSpace(jwtKey))
    faltantes.Add("Jwt:Key");
else if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    faltantes.Add("Jwt:Key (existe pero mide menos de 32 bytes; HMAC-SHA256 necesita al menos eso)");

if (faltantes.Count > 0)
{
    // Mensaje legible en la consola en vez de un volcado de excepcion:
    // quien arranca la API necesita saber QUE comando ejecutar, no en que
    // linea de Program.cs se lanzo la excepcion.
    var entorno = builder.Environment.EnvironmentName;
    var esDev = builder.Environment.IsDevelopment();

    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine();
    Console.Error.WriteLine("======================================================================");
    Console.Error.WriteLine(" LA API NO PUEDE ARRANCAR: falta configuracion");
    Console.Error.WriteLine("======================================================================");
    Console.ResetColor();
    Console.Error.WriteLine($" Entorno actual : {entorno}");
    Console.Error.WriteLine($" Falta          : {string.Join(", ", faltantes)}");
    Console.Error.WriteLine();

    if (!esDev)
    {
        Console.Error.WriteLine(" CAUSA MAS PROBABLE");
        Console.Error.WriteLine($" El entorno es '{entorno}', no 'Development'. Los user-secrets solo se");
        Console.Error.WriteLine(" cargan en Development, por eso no se encuentran los valores.");
        Console.Error.WriteLine();
        Console.Error.WriteLine(" SOLUCION en local (PowerShell):");
        Console.Error.WriteLine("   $env:ASPNETCORE_ENVIRONMENT=\"Development\"");
        Console.Error.WriteLine("   dotnet run");
        Console.Error.WriteLine();
        Console.Error.WriteLine(" O desde Visual Studio: usar el perfil 'http' o 'https' (ya traen");
        Console.Error.WriteLine(" ASPNETCORE_ENVIRONMENT=Development en launchSettings.json).");
    }
    else
    {
        Console.Error.WriteLine(" SOLUCION (PowerShell, dentro de GRUPO_DDR\\GRUPO_DDR_API):");
        if (string.IsNullOrWhiteSpace(connectionString))
            Console.Error.WriteLine("   dotnet user-secrets set \"ConnectionStrings:CadenaSQL\" \"Server=...;User ID=...;Password=...\"");
        if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey ?? "") < 32)
            Console.Error.WriteLine("   dotnet user-secrets set \"Jwt:Key\" \"<clave-larga-y-aleatoria>\"");
        Console.Error.WriteLine();
        Console.Error.WriteLine("   dotnet user-secrets list    # para ver que hay guardado");
    }

    Console.Error.WriteLine();
    Console.Error.WriteLine(" En Azure: App Service > Configuration > Application settings");
    Console.Error.WriteLine("   ConnectionStrings__CadenaSQL   y   Jwt__Key");
    Console.Error.WriteLine();
    Console.Error.WriteLine(" Detalle completo en SEGURIDAD.md");
    Console.Error.WriteLine("======================================================================");
    Console.Error.WriteLine();

    return 1;
}

// Validados arriba: el bloque anterior corta la ejecucion si faltan.
var cadenaSql = connectionString!;
var claveJwt = jwtKey!;

// ---------------------------------------------------------------------
// IMPORTANTE: publicar la clave resuelta en la configuracion.
// UsuarioService.GenerarToken lee IConfiguration["Jwt:Key"] para firmar
// el token. Si la clave salio del archivo .jwt-dev-key (o fue generada),
// solo existia en esta variable local y el login fallaba con
// "Jwt:Key no esta configurada". Al escribirla aqui, el middleware JWT y
// el servicio de login usan exactamente la misma clave.
// Debe ir ANTES de builder.Build().
// ---------------------------------------------------------------------
builder.Configuration["Jwt:Key"] = claveJwt;

// UTF8, no ASCII: ASCII descarta silenciosamente cualquier byte fuera de
// 0-127, lo que debilitaria una clave que contenga caracteres no ASCII.
var keyBytes = Encoding.UTF8.GetBytes(claveJwt);
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "NicaTekAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "NicaTekClients";

// =====================================================================
// SERVICIOS
// =====================================================================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();

// Toda accion exige autenticacion salvo que lleve [AllowAnonymous].
// Antes NINGUN controlador tenia [Authorize]: la API completa estaba
// abierta a internet y el JWT era decorativo.
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API NicaTek", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue aqui el token devuelto por /api/Usuario/Login"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ---------------------------------------------------------------------
// CORS: lista blanca desde configuracion.
// Antes era AllowAnyOrigin(), es decir cualquier sitio del mundo podia
// llamar la API desde el navegador de un usuario logueado.
// ---------------------------------------------------------------------
var origenesPermitidos = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendNicaTek", policy =>
    {
        if (origenesPermitidos.Length > 0)
        {
            policy.WithOrigins(origenesPermitidos)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Sin lista configurada no se abre a todos: se cierra.
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// ---------------------------------------------------------------------
// Base de datos
// ---------------------------------------------------------------------
builder.Services.AddSingleton(new DbConection(cadenaSql));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(cadenaSql));

// ---------------------------------------------------------------------
// Autenticacion JWT
// ---------------------------------------------------------------------
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    // En produccion se exige metadata por HTTPS.
    config.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ClockSkew = TimeSpan.Zero,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = System.Security.Claims.ClaimTypes.Name
    };
});

// ---------------------------------------------------------------------
// Autorizacion por permisos ([RequierePermiso("INV_DELETE")])
// ---------------------------------------------------------------------
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermisoPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermisoHandler>();
builder.Services.AddAuthorization();

// =====================================================================
// INYECCION DE DEPENDENCIAS (repositorios y servicios)
// =====================================================================
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();
builder.Services.AddScoped<IBodegaRepository, BodegaRepository>();
builder.Services.AddScoped<IBodegaService, BodegaService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMarcaRepository, MarcaRepository>();
builder.Services.AddScoped<IMarcaService, MarcaService>();
builder.Services.AddScoped<IModeloRepository, ModeloRepository>();
builder.Services.AddScoped<IModeloService, ModeloService>();
builder.Services.AddScoped<IMarcaModeloRepository, MarcaModeloRepository>();
builder.Services.AddScoped<IMarcaModeloService, MarcaModeloService>();
builder.Services.AddScoped<IDatosPersonalesRepository, DatosPersonalesRepository>();
builder.Services.AddScoped<IDatosPersonalesService, DatosPersonalesService>();
builder.Services.AddScoped<IEstadoRepository, EstadoRepository>();
builder.Services.AddScoped<IEstadoService, EstadoService>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();
builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<ITipoCatalogoRepository, TipoCatalogoRepository>();
builder.Services.AddScoped<ITipoCatalogoService, TipoCatalogoService>();
builder.Services.AddScoped<ITipoMovimientoRepository, TipoMovimientoRepository>();
builder.Services.AddScoped<ITipoMovimientoService, TipoMovimientoService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICorreoRepository, CorreoRepository>();
builder.Services.AddScoped<ICorreoService, CorreoService>();
builder.Services.AddScoped<ITelefonoRepository, TelefonoRepository>();
builder.Services.AddScoped<ITelefonoService, TelefonoService>();
builder.Services.AddScoped<IDireccionRepository, DireccionRepository>();
builder.Services.AddScoped<IDireccionService, DireccionService>();
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IDetalleCompraRepository, DetalleCompraRepository>();
builder.Services.AddScoped<IDetalleCompraService, DetalleCompraService>();
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IDetalleVentaRepository, DetalleVentaRepository>();
builder.Services.AddScoped<IDetalleVentaService, DetalleVentaService>();
builder.Services.AddScoped<IMovimientoInventarioRepository, MovimientoInventarioRepository>();
builder.Services.AddScoped<IMovimientoInventarioService, MovimientoInventarioService>();
builder.Services.AddScoped<IRolPermisoRepository, RolPermisoRepository>();
builder.Services.AddScoped<IRolPermisoService, RolPermisoService>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();
builder.Services.AddScoped<IPermisoService, PermisoService>();
builder.Services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
builder.Services.AddScoped<IBodegaProductoRepository, BodegaProductoRepository>();
builder.Services.AddScoped<IBodegaProductoService, BodegaProductoService>();

var app = builder.Build();

// =====================================================================
// PIPELINE HTTP
// El orden importa: excepciones -> https -> cors -> auth -> endpoints
// =====================================================================
app.UseManejadorDeExcepciones();

// Swagger: habilitado en desarrollo y, en esta instalacion local, tambien
// cuando se arranca sin entorno (queda en Production por defecto). En un
// servidor real se apaga con Seguridad__PublicarSwagger = false.
// Antes se publicaba en la RAIZ de produccion (RoutePrefix = ""),
// regalando el mapa completo de la API a cualquiera.
if (app.Environment.IsDevelopment() ||
    app.Configuration.GetValue("Seguridad:PublicarSwagger", true))
{
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "API NicaTek V1");
        s.RoutePrefix = "swagger";
    });
}
// ---------------------------------------------------------------------
// Redireccion a HTTPS: OPCIONAL, apagada por defecto.
//
// Esta instalacion corre en local por HTTP (el frontend apunta a
// http://localhost:5114/api). Si se forzara la redireccion, arrancar con
// el perfil "https" de Visual Studio haria que cada llamada respondiera
// 307 hacia https://localhost:7162: el navegador la seguiria a otro
// origen y el CORS la rechazaria. Se veria como "la API no funciona".
//
// Para activarla en un servidor real: Seguridad__ForzarHttps = true
// ---------------------------------------------------------------------
if (app.Configuration.GetValue<bool>("Seguridad:ForzarHttps"))
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors("FrontendNicaTek");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// El bloque de validacion de configuracion hace 'return 1', lo que obliga
// a que el punto de entrada devuelva un codigo de salida en todas las rutas.
return 0;