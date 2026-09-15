using System.ComponentModel.DataAnnotations;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Authors;

public class CreateModel : PageModel
{
    private readonly IAuthorApiClient _authorApiClient;

    public CreateModel(IAuthorApiClient authorApiClient)
    {
        _authorApiClient = authorApiClient;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(200, ErrorMessage = "Author name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Biography cannot exceed 2000 characters.")]
        public string? Biography { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var request = new CreateAuthorRequest(Input.Name.Trim(), string.IsNullOrWhiteSpace(Input.Biography) ? null : Input.Biography.Trim());
            var created = await _authorApiClient.CreateAuthorAsync(request);

            TempData["SuccessMessage"] = $"Author '{created?.Name ?? Input.Name}' added successfully!";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while adding the author.";
            return Page();
        }
    }
}
