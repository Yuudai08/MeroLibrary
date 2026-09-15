using System.Security.Claims;
using BookLibrary.Api.Controllers;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookLibrary.Tests;

public class BooksControllerTests
{
    private readonly Mock<IBookReader> _mockReader;
    private readonly Mock<IBookWriter> _mockWriter;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockReader = new Mock<IBookReader>();
        _mockWriter = new Mock<IBookWriter>();
        _controller = new BooksController(_mockReader.Object, _mockWriter.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testuser")
        }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsOkWithBook()
    {
        // Arrange
        var bookId = 1;
        var expectedBook = new BookDto(
            bookId, "Clean Code", "978-0132350884", 1, "Robert C. Martin", 1, "Programming", 2008, BookStatus.Reading, 1, 1, DateTime.UtcNow, null);

        _mockReader.Setup(r => r.GetByIdAsync(bookId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _controller.GetById(bookId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedBook = Assert.IsType<BookDto>(okResult.Value);
        Assert.Equal(bookId, returnedBook.Id);
        Assert.Equal("Clean Code", returnedBook.Title);
        Assert.Equal(1, returnedBook.BookNumber);
    }

    [Fact]
    public async Task GetById_MissingId_ReturnsNotFound()
    {
        // Arrange
        var bookId = 999;
        _mockReader.Setup(r => r.GetByIdAsync(bookId, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookDto?)null);

        // Act
        var result = await _controller.GetById(bookId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateBookRequest("", "123", 1, 1, 2020, BookStatus.WantToRead, null);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.NotNull(badRequestResult.Value);
    }

    [Fact]
    public async Task Create_ValidRequestWithBookNumber_ReturnsCreated()
    {
        // Arrange
        var request = new CreateBookRequest("Clean Architecture", "978-0134494166", 1, 1, 2017, BookStatus.Reading, 2);
        var expectedBook = new BookDto(2, "Clean Architecture", "978-0134494166", 1, "Robert C. Martin", 1, "Programming", 2017, BookStatus.Reading, 1, 2, DateTime.UtcNow, null);

        _mockWriter.Setup(w => w.CreateAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _mockReader.Setup(r => r.GetByIdAsync(2, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedBook = Assert.IsType<BookDto>(createdResult.Value);
        Assert.Equal(2, returnedBook.Id);
        Assert.Equal(2, returnedBook.BookNumber);
    }
}
