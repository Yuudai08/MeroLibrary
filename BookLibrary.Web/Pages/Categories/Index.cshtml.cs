using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ICategoryApiClient _categoryApiClient;

    public IndexModel(ICategoryApiClient categoryApiClient)
    {
        _categoryApiClient = categoryApiClient;
    }

    public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _categoryApiClient.GetAllCategoriesAsync();
    }
}
