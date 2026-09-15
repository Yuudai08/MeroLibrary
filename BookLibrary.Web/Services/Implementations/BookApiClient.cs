using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;

namespace BookLibrary.Web.Services.Implementations;

public class BookApiClient : BaseApiClient, IBookApiClient
{
    private readonly HttpClient _client;

    public BookApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<PagedResult<BookDto>> GetBooksAsync(
        BookStatus? status = null,
        int? authorId = null,
        int? categoryId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();
            if (status.HasValue) queryParams.Add($"status={(int)status.Value}");
            if (authorId.HasValue) queryParams.Add($"authorId={authorId.Value}");
            if (categoryId.HasValue) queryParams.Add($"categoryId={categoryId.Value}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var url = "api/books?" + string.Join("&", queryParams);
            var response = await _client.GetAsync(url, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);

            var result = await response.Content.ReadFromJsonAsync<PagedResult<BookDto>>(JsonOptions, cancellationToken);
            return result ?? new PagedResult<BookDto>(Array.Empty<BookDto>(), page, pageSize, 0, 1);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/books/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<BookDto>(JsonOptions, cancellationToken);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<IReadOnlyList<BookDto>> SearchBooksAsync(string term, CancellationToken cancellationToken = default)
    {
        try
        {
            var encodedTerm = UrlEncoder.Default.Encode(term);
            var response = await _client.GetAsync($"api/books/search?term={encodedTerm}", cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);

            var books = await response.Content.ReadFromJsonAsync<List<BookDto>>(JsonOptions, cancellationToken);
            return books ?? new List<BookDto>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<BookDto?> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/books", request, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<BookDto>(JsonOptions, cancellationToken);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> UpdateBookAsync(int id, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PutAsJsonAsync($"api/books/{id}", request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.DeleteAsync($"api/books/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }
}
