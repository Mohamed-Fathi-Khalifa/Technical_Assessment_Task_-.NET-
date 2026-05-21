using Application.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Backend_.NET_Developer___Technical_Assessment_Task.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            KeyNotFoundException   => (HttpStatusCode.NotFound,            "The requested resource was not found."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,   "You are not authorized to perform this action."),
            ArgumentException      => (HttpStatusCode.BadRequest,          "Invalid argument provided."),
            InvalidOperationException => (HttpStatusCode.BadRequest,       "The operation is not valid in the current state."),
            _                      => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        var errors = new List<string> { exception.Message };

        // Flatten inner exceptions for validation-style errors
        if (exception.InnerException is not null)
            errors.Add(exception.InnerException.Message);

        var response = ApiResponse<object>.Fail(message, errors);

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }),
            cancellationToken);

        return true; // exception handled — do not propagate
    }
}
