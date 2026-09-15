using System.Net.Http.Json;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Interfaces;

namespace BookLibrary.Web.Services.Implementations;

public class AuthApiClient : BaseApiClient, IAuthApiClient
{
    private readonly HttpClient _client;

    public AuthApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/auth/login", request, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions, cancellationToken);
            return result ?? throw new ApiException(500, "Failed to parse authentication response.");
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("api/auth/register", request, cancellationToken);
            await EnsureSuccessAsync(response, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>(JsonOptions, cancellationToken);
            return result ?? throw new ApiException(500, "Failed to parse registration response.");
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex) { throw ApiUnavailableException(ex); }
    }
}
