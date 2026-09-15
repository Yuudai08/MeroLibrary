using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Categories;

public class DetailsModel : PageModel
{
    private readonly ICategoryApiClient _categoryApiClient;

    public DetailsModel(ICategoryApiClient categoryApiClient)
    {
        _categoryApiClient = categoryApiClient;
    }

    public CategoryDto? Category { get; set; }
    public IReadOnlyList<BookDto> Books { get; set; } = Array.Empty<BookDto>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Category = await _categoryApiClient.GetCategoryByIdAsync(id);
        if (Category == null)
        {
            TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
            return RedirectToPage("Index");
        }

        Books = await _categoryApiClient.GetBooksByCategoryAsync(id);
        return Page();
    }
}
