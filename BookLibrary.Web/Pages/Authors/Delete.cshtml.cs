using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Authors;

public class DeleteModel : PageModel
{
    private readonly IAuthorApiClient _authorApiClient;

    public DeleteModel(IAuthorApiClient authorApiClient)
    {
        _authorApiClient = authorApiClient;
    }

    public AuthorDto? Author { get; set; }
    public IReadOnlyList<BookDto> Books { get; set; } = Array.Empty<BookDto>();
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Author = await _authorApiClient.GetAuthorByIdAsync(id);
        if (Author == null)
        {
            TempData["ErrorMessage"] = $"Author with ID {id} was not found.";
            return RedirectToPage("Index");
        }

        Books = await _authorApiClient.GetBooksByAuthorAsync(id);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteStandardAsync(int id)
    {
        try
        {
            var deleted = await _authorApiClient.DeleteAuthorAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = $"Author with ID {id} was not found.";
                return RedirectToPage("Index");
            }

            TempData["SuccessMessage"] = "Author deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            Author = await _authorApiClient.GetAuthorByIdAsync(id);
            Books = await _authorApiClient.GetBooksByAuthorAsync(id);
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while deleting the author.";
            Author = await _authorApiClient.GetAuthorByIdAsync(id);
            Books = await _authorApiClient.GetBooksByAuthorAsync(id);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeleteWithBooksAsync(int id)
    {
        try
        {
            await _authorApiClient.DeleteAuthorWithBooksAsync(id);
            TempData["SuccessMessage"] = "Author and all associated books were deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            Author = await _authorApiClient.GetAuthorByIdAsync(id);
            Books = await _authorApiClient.GetBooksByAuthorAsync(id);
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred during the cascade deletion.";
            Author = await _authorApiClient.GetAuthorByIdAsync(id);
            Books = await _authorApiClient.GetBooksByAuthorAsync(id);
            return Page();
        }
    }
}
