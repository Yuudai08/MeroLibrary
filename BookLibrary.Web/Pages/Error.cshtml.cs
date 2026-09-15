using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public int HttpErrorCode { get; set; }
    public string ErrorTitle { get; set; } = "Something went wrong";
    public string ErrorDescription { get; set; } = "An unexpected error occurred. Please try again.";

    public void OnGet(int? statusCode)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        HttpErrorCode = statusCode ?? HttpContext.Response.StatusCode;

        (ErrorTitle, ErrorDescription) = HttpErrorCode switch
        {
            404 => ("Page Not Found",
                    "The page you are looking for does not exist. It may have been moved or deleted."),
            400 => ("Bad Request",
                    "The request could not be understood by the server. Please check your input and try again."),
            409 => ("Conflict",
                    "The operation conflicted with existing data (for example, a duplicate ISBN or category name)."),
            503 => ("API Unavailable",
                    "The Book Library API is currently unreachable. Please make sure the API is running and try again."),
            500 => ("Internal Server Error",
                    "An unexpected error occurred on the server. Please try again later."),
            _   => ("Error",
                    "An error occurred while processing your request.")
        };
    }
}
