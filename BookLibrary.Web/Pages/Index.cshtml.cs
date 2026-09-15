using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages;

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

    public int TotalBooksCount { get; set; }
    public int TotalAuthorsCount { get; set; }
    public int TotalCategoriesCount { get; set; }

    public int WantToReadCount { get; set; }
    public int ReadingCount { get; set; }
    public int CompletedCount { get; set; }
    public int DroppedCount { get; set; }

    public IReadOnlyList<BookDto> RecentBooks { get; set; } = Array.Empty<BookDto>();

    public async Task OnGetAsync()
    {
        var booksTask = _bookApiClient.GetBooksAsync(page: 1, pageSize: 5);
        var authorsTask = _authorApiClient.GetAllAuthorsAsync();
        var categoriesTask = _categoryApiClient.GetAllCategoriesAsync();

        var wantTask = _bookApiClient.GetBooksAsync(status: BookStatus.WantToRead, page: 1, pageSize: 1);
        var readingTask = _bookApiClient.GetBooksAsync(status: BookStatus.Reading, page: 1, pageSize: 1);
        var completedTask = _bookApiClient.GetBooksAsync(status: BookStatus.Completed, page: 1, pageSize: 1);
        var droppedTask = _bookApiClient.GetBooksAsync(status: BookStatus.Dropped, page: 1, pageSize: 1);

        await Task.WhenAll(booksTask, authorsTask, categoriesTask, wantTask, readingTask, completedTask, droppedTask);

        var pagedBooks = await booksTask;
        TotalBooksCount = pagedBooks.TotalCount;
        RecentBooks = pagedBooks.Items;

        var authors = await authorsTask;
        TotalAuthorsCount = authors.Count;

        var categories = await categoriesTask;
        TotalCategoriesCount = categories.Count;

        WantToReadCount = (await wantTask).TotalCount;
        ReadingCount = (await readingTask).TotalCount;
        CompletedCount = (await completedTask).TotalCount;
        DroppedCount = (await droppedTask).TotalCount;
    }
}
