using System.ComponentModel.DataAnnotations;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookLibrary.Web.Pages.Books;

public class CreateModel : PageModel
{
    private readonly IBookApiClient _bookApiClient;
    private readonly IAuthorApiClient _authorApiClient;
    private readonly ICategoryApiClient _categoryApiClient;

    public CreateModel(
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
        public int PublishedYear { get; set; } = DateTime.UtcNow.Year;

        [Required(ErrorMessage = "Book status is required.")]
        public BookStatus Status { get; set; } = BookStatus.WantToRead;

        [Display(Name = "Book Number (Optional)")]
        [Range(1, 100000, ErrorMessage = "Book number must be a positive number.")]
        public int? BookNumber { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadDropdownsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return Page();
        }

        try
        {
            var request = new CreateBookRequest(
                Input.Title.Trim(),
                Input.ISBN.Trim(),
                Input.AuthorId,
                Input.CategoryId,
                Input.PublishedYear,
                Input.Status,
                Input.BookNumber);

            var createdBook = await _bookApiClient.CreateBookAsync(request);
            TempData["SuccessMessage"] = $"Book '{createdBook?.Title ?? Input.Title}' created successfully!";
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
            ErrorMessage = "An unexpected error occurred while creating the book. Please try again.";
            await LoadDropdownsAsync();
            return Page();
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
