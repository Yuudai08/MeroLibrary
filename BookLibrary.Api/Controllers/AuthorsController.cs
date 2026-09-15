using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorReader _authorReader;
    private readonly IAuthorWriter _authorWriter;
    private readonly IBookReader _bookReader;

    public AuthorsController(IAuthorReader authorReader, IAuthorWriter authorWriter, IBookReader bookReader)
    {
        _authorReader = authorReader;
        _authorWriter = authorWriter;
        _bookReader = bookReader;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuthorDto>>> GetAll(CancellationToken cancellationToken)
    {
        var authors = await _authorReader.GetAllAsync(cancellationToken);
        return Ok(authors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var author = await _authorReader.GetByIdAsync(id, cancellationToken);
        if (author == null)
        {
            return NotFound();
        }
        return Ok(author);
    }

    [HttpGet("{id:int}/books")]
    public async Task<ActionResult<IReadOnlyList<BookDto>>> GetBooksByAuthor(int id, CancellationToken cancellationToken)
    {
        var author = await _authorReader.GetByIdAsync(id, cancellationToken);
        if (author == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        var books = await _bookReader.GetByAuthorAsync(userId, id, cancellationToken);
        return Ok(books);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : 1;
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Author name is required." });
        }

        var newId = await _authorWriter.CreateAsync(request, cancellationToken);
        var created = await _authorReader.GetByIdAsync(newId, cancellationToken);

        if (created == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve created author.");
        }

        return CreatedAtAction(nameof(GetById), new { id = newId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Author name is required." });
        }

        var updated = await _authorWriter.UpdateAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _authorWriter.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}/with-books")]
    public async Task<IActionResult> DeleteWithBooks(int id, CancellationToken cancellationToken)
    {
        var author = await _authorReader.GetByIdAsync(id, cancellationToken);
        if (author == null)
        {
            return NotFound();
        }

        await _authorWriter.DeleteWithBooksAsync(id, cancellationToken);
        return NoContent();
    }
}
