using System.Net;
using System.Net.Http.Json;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;

namespace BookLibrary.Web.Services.Implementations;

public class CategoryApiClient : BaseApiClient, ICategoryApiClient
{
    private readonly HttpClient _client;

    public CategoryApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync("api/categories", cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>(JsonOptions, cancellationToken);
            return categories ?? new List<CategoryDto>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/categories/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            await EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<CategoryDto>(JsonOptions, cancellationToken);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<IReadOnlyList<BookDto>> GetBooksByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/categories/{categoryId}/books", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return new List<BookDto>();
            await EnsureSuccessAsync(response, cancellationToken);
            var books = await response.Content.ReadFromJsonAsync<List<BookDto>>(JsonOptions, cancellationToken);
            return books ?? new List<BookDto>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/categories", request, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            return await response.Content.ReadFromJsonAsync<CategoryDto>(JsonOptions, cancellationToken);
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PutAsJsonAsync($"api/categories/{id}", request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.DeleteAsync($"api/categories/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound) return false;
            await EnsureSuccessAsync(response, cancellationToken);
            return true;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }
}
