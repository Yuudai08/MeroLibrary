using System.ComponentModel.DataAnnotations;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookLibrary.Web.Pages.Books;

public class EditModel : PageModel
{
    private readonly IBookApiClient _bookApiClient;
    private readonly IAuthorApiClient _authorApiClient;
    private readonly ICategoryApiClient _categoryApiClient;

    public EditModel(
        IBookApiClient bookApiClient,
        IAuthorApiClient authorApiClient,
        ICategoryApiClient categoryApiClient)
    {
        _bookApiClient = bookApiClient;
        _authorApiClient = authorApiClient;
        _categoryApiClient = categoryApiClient;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public int BookId { get; set; }
    public SelectList? AuthorsList { get; set; }
    public SelectList? CategoriesList { get; set; }
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(300, ErrorMessage = "Title cannot exceed 300 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters.")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author selection is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid author.")]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Category selection is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Published year is required.")]
        [Range(1000, 2100, ErrorMessage = "Published year must be between 1000 and 2100.")]
        [Display(Name = "Published Year")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Book status is required.")]
        public BookStatus Status { get; set; }

        [Display(Name = "Book Number (Optional)")]
        [Range(1, 100000, ErrorMessage = "Book number must be a positive number.")]
        public int? BookNumber { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        BookId = id;
        var book = await _bookApiClient.GetBookByIdAsync(id);
        if (book == null)
        {
            TempData["ErrorMessage"] = $"Book with ID {id} was not found.";
            return RedirectToPage("Index");
        }

        Input = new InputModel
        {
            Title = book.Title,
            ISBN = book.ISBN,
            AuthorId = book.AuthorId,
            CategoryId = book.CategoryId,
            PublishedYear = book.PublishedYear,
            Status = book.Status,
            BookNumber = book.BookNumber
        };

        await LoadDropdownsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        BookId = id;

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        try
        {
            // Only allow book number when status is Reading or Dropped
            var bookNumber = (Input.Status == BookStatus.Reading || Input.Status == BookStatus.Dropped)
                ? Input.BookNumber
                : null;

            var request = new UpdateBookRequest(
                Input.Title.Trim(),
                Input.ISBN.Trim(),
                Input.AuthorId,
                Input.CategoryId,
                Input.PublishedYear,
                Input.Status,
                bookNumber);

            var updated = await _bookApiClient.UpdateBookAsync(id, request);
            if (!updated)
            {
                ErrorMessage = "The book could not be found to update.";
                await LoadDropdownsAsync();
                return Page();
            }

            TempData["SuccessMessage"] = $"Book '{Input.Title}' updated successfully!";
            return RedirectToPage("Index");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            await LoadDropdownsAsync();
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "An unexpected error occurred while updating the book. Please try again.";
            await LoadDropdownsAsync();
            return Page();
        }
    }

    public class QuickAuthorInput
    {
        [Required(ErrorMessage = "Author name is required.")]
        public string Name { get; set; } = string.Empty;

        public string? Biography { get; set; }
    }

    public async Task<IActionResult> OnPostQuickCreateAuthorAsync([FromBody] QuickAuthorInput request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
        {
            return BadRequest(new { message = "Author name is required." });
        }

        try
        {
            var created = await _authorApiClient.CreateAuthorAsync(new CreateAuthorRequest(request.Name.Trim(), request.Biography?.Trim()));
            return new JsonResult(created);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while creating the author." });
        }
    }

    public class QuickCategoryInput
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostQuickCreateCategoryAsync([FromBody] QuickCategoryInput request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
        {
            return BadRequest(new { message = "Category name is required." });
        }

        try
        {
            var created = await _categoryApiClient.CreateCategoryAsync(new CreateCategoryRequest(request.Name.Trim()));
            return new JsonResult(created);
        }
        catch (ApiException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while creating the category." });
        }
    }

    private async Task LoadDropdownsAsync()
    {
        var authors = await _authorApiClient.GetAllAuthorsAsync();
        AuthorsList = new SelectList(authors, nameof(AuthorDto.Id), nameof(AuthorDto.Name));

        var categories = await _categoryApiClient.GetAllCategoriesAsync();
        CategoriesList = new SelectList(categories, nameof(CategoryDto.Id), nameof(CategoryDto.Name));
    }
}
