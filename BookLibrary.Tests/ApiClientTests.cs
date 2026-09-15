using System.Net;
using System.Text.Json;
using BookLibrary.Web.Models;
using BookLibrary.Web.Services.Implementations;
using Moq;
using Moq.Protected;
using Xunit;

namespace BookLibrary.Tests;

public class ApiClientTests
{
    private (HttpClient client, Mock<HttpMessageHandler> handlerMock) CreateMockHttpClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response)
            .Verifiable();

        var client = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5288/")
        };

        return (client, handlerMock);
    }

    [Fact]
    public async Task BookApiClient_GetBooksAsync_ReturnsPagedResult()
    {
        // Arrange
        var pagedData = new PagedResult<BookDto>(
            new List<BookDto>
            {
                new(1, "Test Book", "123456", 1, "Author A", 1, "Category C", 2024, BookStatus.Reading, DateTime.UtcNow, null)
            },
            1, 10, 1, 1);

        var json = JsonSerializer.Serialize(pagedData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        var (httpClient, _) = CreateMockHttpClient(response);
        var apiClient = new BookApiClient(httpClient);

        // Act
        var result = await apiClient.GetBooksAsync(BookStatus.Reading, 1, 1, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Test Book", result.Items[0].Title);
    }

    [Fact]
    public async Task BookApiClient_GetBookById_MissingId_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var (httpClient, _) = CreateMockHttpClient(response);
        var apiClient = new BookApiClient(httpClient);

        // Act
        var result = await apiClient.GetBookByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AuthorApiClient_DeleteAuthorWithBooks_InvokesCascadeEndpoint()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var (httpClient, handlerMock) = CreateMockHttpClient(response);
        var apiClient = new AuthorApiClient(httpClient);

        // Act
        var success = await apiClient.DeleteAuthorWithBooksAsync(5);

        // Assert
        Assert.True(success);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete && req.RequestUri!.AbsolutePath == "/api/authors/5/with-books"),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task CategoryApiClient_CreateCategory_DuplicateName_ThrowsApiExceptionWith409()
    {
        // Arrange
        var errorJson = JsonSerializer.Serialize(new { status = 409, error = "Conflict", message = "Category name already exists." });
        var response = new HttpResponseMessage(HttpStatusCode.Conflict)
        {
            Content = new StringContent(errorJson, System.Text.Encoding.UTF8, "application/json")
        };

        var (httpClient, _) = CreateMockHttpClient(response);
        var apiClient = new CategoryApiClient(httpClient);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ApiException>(() => apiClient.CreateCategoryAsync(new CreateCategoryRequest("Programming")));
        Assert.Equal(409, ex.StatusCode);
        Assert.Equal("Category name already exists.", ex.Message);
    }

    [Fact]
    public async Task ApiClient_NetworkFailure_Throws503ApiException()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Connection refused"));

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5288/")
        };
        var apiClient = new BookApiClient(httpClient);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ApiException>(() => apiClient.GetBooksAsync());
        Assert.Equal(503, ex.StatusCode);
        Assert.Contains("Could not connect to the MeroLibrary API", ex.Message);
    }
}
