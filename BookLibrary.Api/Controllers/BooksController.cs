using System.Security.Claims;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookReader _reader;
    private readonly IBookWriter _writer;

    public BooksController(IBookReader reader, IBookWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<BookDto>>> GetAll(
        [FromQuery] BookStatus? status,
        [FromQuery] int? authorId,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var userId = GetCurrentUserId();
        var pagedResult = await _reader.GetPagedAsync(userId, status, authorId, categoryId, page, pageSize, cancellationToken);
        return Ok(pagedResult);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var book = await _reader.GetByIdAsync(id, userId, cancellationToken);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(book);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<BookDto>>> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Search term cannot be empty.");
        }

        var userId = GetCurrentUserId();
        var books = await _reader.SearchAsync(userId, term, cancellationToken);
        return Ok(books);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookRequest request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateRequest(request.Title, request.ISBN, request.PublishedYear, request.Status, request.AuthorId, request.CategoryId);
        if (validationResult != null)
        {
            return validationResult;
        }

        // Only books with Status 'Reading' or 'Dropped' can have a BookNumber
        if (request.Status != Domain.BookStatus.Reading && request.Status != Domain.BookStatus.Dropped && request.BookNumber.HasValue)
        {
            request = request with { BookNumber = null };
        }

        var userId = GetCurrentUserId();
        var newId = await _writer.CreateAsync(userId, request, cancellationToken);
        var createdBook = await _reader.GetByIdAsync(newId, userId, cancellationToken);

        if (createdBook == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve created book.");
        }

        return CreatedAtAction(nameof(GetById), new { id = newId }, createdBook);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateRequest(request.Title, request.ISBN, request.PublishedYear, request.Status, request.AuthorId, request.CategoryId);
        if (validationResult != null)
        {
            return validationResult;
        }

        // Only books with Status 'Reading' or 'Dropped' can have a BookNumber
        if (request.Status != Domain.BookStatus.Reading && request.Status != Domain.BookStatus.Dropped && request.BookNumber.HasValue)
        {
            request = request with { BookNumber = null };
        }

        var userId = GetCurrentUserId();
        var updated = await _writer.UpdateAsync(id, userId, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var deleted = await _writer.DeleteAsync(id, userId, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(claim, out var userId))
        {
            return userId;
        }
        return 1;
    }

    private static BadRequestObjectResult? ValidateRequest(string title, string isbn, int publishedYear, BookStatus status, int authorId, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return new BadRequestObjectResult(new { message = "Book title is required." });
        if (string.IsNullOrWhiteSpace(isbn))
            return new BadRequestObjectResult(new { message = "Book ISBN is required." });
        if (authorId <= 0)
            return new BadRequestObjectResult(new { message = "Valid AuthorId is required." });
        if (categoryId <= 0)
            return new BadRequestObjectResult(new { message = "Valid CategoryId is required." });
        if (publishedYear < 1000 || publishedYear > DateTime.UtcNow.Year + 1)
            return new BadRequestObjectResult(new { message = "Published year is invalid." });
        if (!Enum.IsDefined(status))
            return new BadRequestObjectResult(new { message = "Book status is invalid." });
        return null;
    }
}
