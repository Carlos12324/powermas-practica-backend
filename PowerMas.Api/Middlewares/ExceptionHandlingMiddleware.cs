using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace PowerMas.Api.Middlewares;

/// <summary>
/// Middleware para manejo global de excepciones
/// Mapea errores de SQL Server (THROW) a respuestas HTTP apropiadas
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

        var problemDetails = CreateProblemDetails(context, exception);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception)
    {
        // Verificar si es una SqlException con codigos de error específicos
        if (exception is SqlException sqlException)
        {
            return MapSqlExceptionToProblemDetails(context, sqlException);
        }

        // Excepcion generica
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Error interno del servidor",
            Detail = "Ha ocurrido un error inesperado. Por favor, intente nuevamente.",
            Instance = context.Request.Path
        };
    }

    private ProblemDetails MapSqlExceptionToProblemDetails(HttpContext context, SqlException sqlException)
    {
        // Codigos de error del SP segun el init.sql:
        // 50001 => DocumentoIdentidadId no existe
        // 50002 => NumeroDocumento no cumple la longitud
        // 50003 => NumeroDocumento debe contener solo numeros
        // 50004 => Beneficiario no existe (Update)
        // 50005 => Beneficiario no existe (Delete)
        // 50006 => Beneficiario no existe (GetById)

        var errorCode = sqlException.Number;
        var message = sqlException.Message;

        return errorCode switch
        {
            // Errores de validación de documento -> 400 Bad Request
            50001 or 50002 or 50003 => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Error de validación",
                Detail = message,
                Instance = context.Request.Path
            },

            // Beneficiario no existe -> 404 Not Found
            50004 or 50005 or 50006 => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Recurso no encontrado",
                Detail = "Beneficiario no existe.",
                Instance = context.Request.Path
            },

            // Otros errores SQL -> 500 Internal Server Error
            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Error de base de datos",
                Detail = "Ha ocurrido un error en la base de datos.",
                Instance = context.Request.Path
            }
        };
    }
}

/// <summary>
/// Metodo de extension para registrar el middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
