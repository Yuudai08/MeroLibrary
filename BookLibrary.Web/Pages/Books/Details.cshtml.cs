using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Books;

public class DetailsModel : PageModel
{
    private readonly IBookApiClient _bookApiClient;

    public DetailsModel(IBookApiClient bookApiClient)
    {
        _bookApiClient = bookApiClient;
    }

    public BookDto? Book { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Book = await _bookApiClient.GetBookByIdAsync(id);
        if (Book == null)
        {
            TempData["ErrorMessage"] = $"Book with ID {id} was not found.";
            return RedirectToPage("Index");
        }

        return Page();
    }
}
