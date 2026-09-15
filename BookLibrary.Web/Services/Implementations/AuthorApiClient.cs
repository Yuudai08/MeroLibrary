using System.Net;
using System.Net.Http.Json;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;

namespace BookLibrary.Web.Services.Implementations;

public class AuthorApiClient : BaseApiClient, IAuthorApiClient
{
    private readonly HttpClient _client;

    public AuthorApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync("api/authors", cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>(JsonOptions, cancellationToken);
            return authors ?? new List<AuthorDto>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<AuthorDto?> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/authors/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<AuthorDto>(JsonOptions, cancellationToken);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<IReadOnlyList<BookDto>> GetBooksByAuthorAsync(int authorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/authors/{authorId}/books", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return new List<BookDto>();
            await EnsureSuccessAsync(response, cancellationToken);
            var books = await response.Content.ReadFromJsonAsync<List<BookDto>>(JsonOptions, cancellationToken);
            return books ?? new List<BookDto>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<AuthorDto?> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/authors", request, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<AuthorDto>(JsonOptions, cancellationToken);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> UpdateAuthorAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PutAsJsonAsync($"api/authors/{id}", request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> DeleteAuthorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.DeleteAsync($"api/authors/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> DeleteAuthorWithBooksAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.DeleteAsync($"api/authors/{id}/with-books", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }
}
