using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Authors;

public class DetailsModel : PageModel
{
    private readonly IAuthorApiClient _authorApiClient;

    public DetailsModel(IAuthorApiClient authorApiClient)
    {
        _authorApiClient = authorApiClient;
    }

    public AuthorDto? Author { get; set; }
    public IReadOnlyList<BookDto> Books { get; set; } = Array.Empty<BookDto>();

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
}
