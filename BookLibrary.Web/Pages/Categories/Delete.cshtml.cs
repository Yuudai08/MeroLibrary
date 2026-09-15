using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Categories;

public class DeleteModel : PageModel
{
    private readonly ICategoryApiClient _categoryApiClient;

    public DeleteModel(ICategoryApiClient categoryApiClient)
    {
        _categoryApiClient = categoryApiClient;
    }

    public CategoryDto? Category { get; set; }
    public IReadOnlyList<BookDto> Books { get; set; } = Array.Empty<BookDto>();
    public string? ErrorMessage { get; set; }

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

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            var deleted = await _categoryApiClient.DeleteCategoryAsync(id);
            if (!deleted)
            {
                TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
                return RedirectToPage("Index");
            }

            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            Category = await _categoryApiClient.GetCategoryByIdAsync(id);
            Books = await _categoryApiClient.GetBooksByCategoryAsync(id);
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while deleting the category.";
            Category = await _categoryApiClient.GetCategoryByIdAsync(id);
            Books = await _categoryApiClient.GetBooksByCategoryAsync(id);
            return Page();
        }
    }
}
