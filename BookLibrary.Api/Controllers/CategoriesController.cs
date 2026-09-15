using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryReader _categoryReader;
    private readonly ICategoryWriter _categoryWriter;
    private readonly IBookReader _bookReader;

    public CategoriesController(ICategoryReader categoryReader, ICategoryWriter categoryWriter, IBookReader bookReader)
    {
        _categoryReader = categoryReader;
        _categoryWriter = categoryWriter;
        _bookReader = bookReader;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _categoryReader.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryReader.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpGet("{id:int}/books")]
    public async Task<ActionResult<IReadOnlyList<BookDto>>> GetBooksByCategory(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryReader.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return NotFound();
        }

        var userId = GetCurrentUserId();
        var books = await _bookReader.GetByCategoryAsync(userId, id, cancellationToken);
        return Ok(books);
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var userId) ? userId : 1;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Category name is required." });
        }

        var newId = await _categoryWriter.CreateAsync(request, cancellationToken);
        var created = await _categoryReader.GetByIdAsync(newId, cancellationToken);

        if (created == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to retrieve created category.");
        }

        return CreatedAtAction(nameof(GetById), new { id = newId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Category name is required." });
        }

        var updated = await _categoryWriter.UpdateAsync(id, request, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _categoryWriter.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
