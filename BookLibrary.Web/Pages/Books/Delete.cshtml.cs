using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Books;

public class DeleteModel : PageModel
{
    private readonly IBookApiClient _bookApiClient;

    public DeleteModel(IBookApiClient bookApiClient)
    {
        _bookApiClient = bookApiClient;
    }

    public BookDto? Book { get; set; }
    public string? ErrorMessage { get; set; }

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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _bookApiClient.DeleteBookAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = $"Book with ID {id} was not found.";
                return RedirectToPage("Index");
            }

            TempData["SuccessMessage"] = "Book deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            Book = await _bookApiClient.GetBookByIdAsync(id);
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while deleting the book.";
            Book = await _bookApiClient.GetBookByIdAsync(id);
            return Page();
        }
    }
}
