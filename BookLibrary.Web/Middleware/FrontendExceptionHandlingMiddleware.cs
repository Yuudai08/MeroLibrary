using BookLibrary.Web.Models;

namespace BookLibrary.Web.Middleware;

/// <summary>
/// Catches unhandled exceptions in the Razor Pages pipeline and
/// renders a user-friendly error response instead of a raw stack trace.
/// This is separate from the API's ExceptionHandlingMiddleware — that one
/// protects the API endpoint; this one protects the frontend web app.
/// </summary>
public class FrontendExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<FrontendExceptionHandlingMiddleware> _logger;

    public FrontendExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<FrontendExceptionHandlingMiddleware> logger)
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
        catch (ApiException ex)
        {
            _logger.LogWarning("API returned error {StatusCode}: {Message}", ex.StatusCode, ex.Message);

            if (ex.StatusCode == 401)
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            context.Response.StatusCode = ex.StatusCode;
            context.Response.Redirect($"/Error?statusCode={ex.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the BookLibrary API.");
            context.Response.Redirect("/Error?statusCode=503");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in BookLibrary.Web pipeline.");
            context.Response.Redirect("/Error?statusCode=500");
        }
    }
}
