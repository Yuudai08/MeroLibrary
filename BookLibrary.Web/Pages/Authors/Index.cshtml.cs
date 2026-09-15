using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookLibrary.Web.Pages.Authors;

public class IndexModel : PageModel
{
    private readonly IAuthorApiClient _authorApiClient;

    public IndexModel(IAuthorApiClient authorApiClient)
    {
        _authorApiClient = authorApiClient;
    }

    public IReadOnlyList<AuthorDto> Authors { get; set; } = Array.Empty<AuthorDto>();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Authors = await _authorApiClient.GetAllAuthorsAsync();
    }
}
