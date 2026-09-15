using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BookLibrary.Web.Models;

namespace BookLibrary.Web.Services.Implementations;

public abstract class BaseApiClient
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        if (response.IsSuccessStatusCode)
            return;

        var message = await ExtractErrorMessageAsync(response, cancellationToken);
        throw new ApiException((int)response.StatusCode, message);
    }

    protected static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                return response.ReasonPhrase ?? $"HTTP Error {(int)response.StatusCode}";
            }

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("message", out var messageProp) && messageProp.ValueKind == JsonValueKind.String)
                {
                    return messageProp.GetString()!;
                }
                if (root.TryGetProperty("error", out var errorProp) && errorProp.ValueKind == JsonValueKind.String)
                {
                    return errorProp.GetString()!;
                }
            }

            return content;
        }
        catch
        {
            return response.ReasonPhrase ?? $"HTTP Error {(int)response.StatusCode}";
        }
    }

    /// <summary>
    /// Wraps HttpRequestException (connection refused, DNS failure, timeout, etc.)
    /// into an ApiException with a friendly 503 status code so the frontend error
    /// middleware and page models can surface "API unavailable" to the user clearly.
    /// </summary>
    protected static ApiException ApiUnavailableException(HttpRequestException ex)
        => new(503, $"Could not connect to the MeroLibrary API. Please ensure the API is running. ({ex.Message})");
}
