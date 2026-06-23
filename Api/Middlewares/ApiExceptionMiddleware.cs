using Microsoft.AspNetCore.Mvc;
using Sanaclub.Application.Common.Exceptions;

namespace Sanaclub.Api.Middlewares;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(
                    exception,
                    "Ocurrió una excepción después de iniciar la respuesta. TraceId: {TraceId}. Path: {Path}. Method: {Method}",
                    context.TraceIdentifier,
                    context.Request.Path,
                    context.Request.Method);

                throw;
            }

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            AppValidationException => (
                StatusCodes.Status400BadRequest,
                "Error de validación",
                exception.Message),

            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                exception.Message),

            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflicto en la solicitud",
                exception.Message),

            ForbiddenException => (
                StatusCodes.Status403Forbidden,
                "Acceso denegado",
                exception.Message),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "No autenticado",
                "Debe autenticarse para acceder a este recurso."),

            BadHttpRequestException => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                "La solicitud enviada no es válida."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado. Intente nuevamente más tarde.")
        };

        LogException(context, exception, statusCode);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        if (exception is AppValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private void LogException(HttpContext context, Exception exception, int statusCode)
    {
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Error no controlado. TraceId: {TraceId}. Path: {Path}. Method: {Method}",
                context.TraceIdentifier,
                context.Request.Path,
                context.Request.Method);

            return;
        }

        _logger.LogWarning(
            exception,
            "Error controlado. StatusCode: {StatusCode}. TraceId: {TraceId}. Path: {Path}. Method: {Method}",
            statusCode,
            context.TraceIdentifier,
            context.Request.Path,
            context.Request.Method);
    }
}
