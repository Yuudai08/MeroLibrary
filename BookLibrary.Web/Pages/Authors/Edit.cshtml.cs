using System.ComponentModel.DataAnnotations;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Authors;

public class EditModel : PageModel
{
    private readonly IAuthorApiClient _authorApiClient;

    public EditModel(IAuthorApiClient authorApiClient)
    {
        _authorApiClient = authorApiClient;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int AuthorId { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(200, ErrorMessage = "Author name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Biography cannot exceed 2000 characters.")]
        public string? Biography { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        AuthorId = id;
        var author = await _authorApiClient.GetAuthorByIdAsync(id);
        if (author == null)
        {
            TempData["ErrorMessage"] = $"Author with ID {id} was not found.";
            return RedirectToPage("Index");
        }

        Input = new InputModel
        {
            Name = author.Name,
            Biography = author.Biography
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        AuthorId = id;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var request = new UpdateAuthorRequest(Input.Name.Trim(), string.IsNullOrWhiteSpace(Input.Biography) ? null : Input.Biography.Trim());
            var updated = await _authorApiClient.UpdateAuthorAsync(id, request);
            if (!updated)
            {
                TempData["ErrorMessage"] = $"Author with ID {id} was not found.";
                return RedirectToPage("Index");
            }

            TempData["SuccessMessage"] = $"Author '{Input.Name}' updated successfully!";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while updating the author.";
            return Page();
        }
    }
}
