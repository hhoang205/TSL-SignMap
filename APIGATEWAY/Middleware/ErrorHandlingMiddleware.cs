using System.Net;
using System.Text.Json;
using ApiGateway.Models;

namespace ApiGateway.Middleware;


/// Middleware để handle errors globally và transform thành standard response format

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment environment)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.Items["RequestId"]?.ToString() ?? Guid.NewGuid().ToString();
        
        _logger.LogError(exception, "Error occurred. RequestId: {RequestId}", requestId);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Error = new ErrorDetail
            {
                Code = GetErrorCode(exception),
                Message = GetUserFriendlyMessage(exception),
                Details = _environment.IsDevelopment() ? exception.ToString() : null,
                Timestamp = DateTime.UtcNow,
                RequestId = requestId
            }
        };

        response.StatusCode = GetStatusCode(exception);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "UNAUTHORIZED",
            ArgumentException => "VALIDATION_ERROR",
            KeyNotFoundException => "NOT_FOUND",
            InvalidOperationException => "INVALID_OPERATION",
            TimeoutException => "TIMEOUT",
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => "You are not authorized to perform this action.",
            ArgumentException => "Invalid request parameters.",
            KeyNotFoundException => "The requested resource was not found.",
            InvalidOperationException => "The operation cannot be completed.",
            TimeoutException => "The request timed out. Please try again.",
            _ => "An error occurred while processing your request."
        };
    }
}

