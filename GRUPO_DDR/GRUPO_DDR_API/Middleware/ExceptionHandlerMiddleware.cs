using System.Net;
using System.Text.Json;

namespace GRUPO_DDR_API.Middleware
{
    /// <summary>
    /// Manejo central de errores.
    ///
    /// Reemplaza los ~200 bloques try/catch repetidos en los controladores
    /// que devolvian 'error = ex.Message' al cliente. Eso filtraba nombres
    /// de procedimientos, columnas y errores crudos de SQL Server a
    /// cualquiera que llamara la API.
    ///
    /// Ahora: el detalle completo va al log del servidor, el cliente
    /// recibe un mensaje generico y un identificador de traza para poder
    /// cruzarlo con el log si reporta el problema.
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ManejarAsync(context, ex);
            }
        }

        private async Task ManejarAsync(HttpContext context, Exception ex)
        {
            var trazaId = context.TraceIdentifier;

            _logger.LogError(ex,
                "Error no controlado. TrazaId={TrazaId} Ruta={Metodo} {Ruta} Usuario={Usuario}",
                trazaId,
                context.Request.Method,
                context.Request.Path,
                context.User?.Identity?.Name ?? "anonimo");

            if (context.Response.HasStarted)
            {
                // Ya se empezo a escribir la respuesta: no se puede
                // reemplazar sin corromperla.
                return;
            }

            // ReglaNegocioException lleva mensajes escritos para el usuario
            // final, por eso si se devuelven tal cual.
            var (estado, mensaje) = ex switch
            {
                ReglaNegocioException rn => (HttpStatusCode.BadRequest, rn.Message),
                UnauthorizedAccessException => (HttpStatusCode.Forbidden, "No tiene permiso para realizar esta accion."),
                KeyNotFoundException => (HttpStatusCode.NotFound, "El recurso solicitado no existe."),
                TaskCanceledException => (HttpStatusCode.RequestTimeout, "La operacion tardo demasiado. Intente de nuevo."),
                _ => (HttpStatusCode.InternalServerError, "Ocurrio un error procesando la solicitud.")
            };

            context.Response.Clear();
            context.Response.StatusCode = (int)estado;
            context.Response.ContentType = "application/json";

            var cuerpo = new Dictionary<string, object?>
            {
                ["exito"] = false,
                ["mensaje"] = mensaje,
                ["trazaId"] = trazaId
            };

            // El detalle tecnico solo en desarrollo. Nunca en produccion.
            if (_env.IsDevelopment() && estado == HttpStatusCode.InternalServerError)
            {
                cuerpo["detalleDesarrollo"] = ex.ToString();
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(cuerpo,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }

    /// <summary>
    /// Error de negocio esperado (stock insuficiente, cliente inexistente).
    /// Su mensaje esta escrito para mostrarse al usuario tal cual.
    /// </summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseManejadorDeExcepciones(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
