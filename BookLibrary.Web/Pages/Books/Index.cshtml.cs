using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookLibrary.Web.Pages.Books;

public class IndexModel : PageModel
{
    private readonly IBookApiClient _bookApiClient;
    private readonly IAuthorApiClient _authorApiClient;
    private readonly ICategoryApiClient _categoryApiClient;

    public IndexModel(
        IBookApiClient bookApiClient,
        IAuthorApiClient authorApiClient,
        ICategoryApiClient categoryApiClient)
    {
        _bookApiClient = bookApiClient;
        _authorApiClient = authorApiClient;
        _categoryApiClient = categoryApiClient;
    }

    public PagedResult<BookDto>? PagedBooks { get; set; }
    public IReadOnlyList<BookDto>? SearchResults { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public BookStatus? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? AuthorId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public SelectList? AuthorsList { get; set; }
    public SelectList? CategoriesList { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public bool HasActiveFilters =>
        !string.IsNullOrWhiteSpace(SearchTerm) ||
        Status.HasValue ||
        AuthorId.HasValue ||
        CategoryId.HasValue;

    public async Task OnGetAsync()
    {
        if (PageNumber < 1) PageNumber = 1;
        if (PageSize < 1) PageSize = 10;
        if (PageSize > 100) PageSize = 100;

        var authorsTask = _authorApiClient.GetAllAuthorsAsync();
        var categoriesTask = _categoryApiClient.GetAllCategoriesAsync();

        await Task.WhenAll(authorsTask, categoriesTask);

        var authors = await authorsTask;
        AuthorsList = new SelectList(authors, nameof(AuthorDto.Id), nameof(AuthorDto.Name), AuthorId);

        var categories = await categoriesTask;
        CategoriesList = new SelectList(categories, nameof(CategoryDto.Id), nameof(CategoryDto.Name), CategoryId);

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            SearchResults = await _bookApiClient.SearchBooksAsync(SearchTerm.Trim());
        }
        else
        {
            PagedBooks = await _bookApiClient.GetBooksAsync(Status, AuthorId, CategoryId, PageNumber, PageSize);
        }
    }
}
