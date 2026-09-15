using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace BookLibrary.Api.Middleware;

// Global exception handling middleware catching unhandled exceptions and database constraints,
// returning structured JSON error responses with appropriate HTTP status codes (400, 409, 500).
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
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        var sqlEx = exception.GetBaseException() as SqlException ?? exception as SqlException;

        if (sqlEx != null)
        {
            // 2601 / 2627: Unique constraint violation (e.g. duplicate ISBN)
            // 547: Foreign key or check constraint violation (e.g. invalid Author/Category or Status)
            if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
            {
                statusCode = HttpStatusCode.Conflict;
                message = "Conflict: A record with the same unique value (such as ISBN) already exists.";
            }
            else if (sqlEx.Number == 547)
            {
                statusCode = HttpStatusCode.BadRequest;
                message = "Bad Request: Database constraint violation (invalid Foreign Key reference or Status value).";
            }
        }
        else if (exception is ArgumentException || exception is InvalidOperationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = exception.Message;
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            error = statusCode.ToString(),
            message = message
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
