using System.ComponentModel.DataAnnotations;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Categories;

public class EditModel : PageModel
{
    private readonly ICategoryApiClient _categoryApiClient;

    public EditModel(ICategoryApiClient categoryApiClient)
    {
        _categoryApiClient = categoryApiClient;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int CategoryId { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        CategoryId = id;
        var category = await _categoryApiClient.GetCategoryByIdAsync(id);
        if (category == null)
        {
            TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
            return RedirectToPage("Index");
        }

        Input = new InputModel
        {
            Name = category.Name
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        CategoryId = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var request = new UpdateCategoryRequest(Input.Name.Trim());
            var updated = await _categoryApiClient.UpdateCategoryAsync(id, request);
            if (!updated)
            {
                TempData["ErrorMessage"] = $"Category with ID {id} was not found.";
                return RedirectToPage("Index");
            }

            TempData["SuccessMessage"] = $"Category '{Input.Name}' updated successfully!";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while updating the category.";
            return Page();
        }
    }
}
