using BookLibrary.Application.DTOs;
using BookLibrary.Domain;

namespace BookLibrary.Application.Interfaces;

// Query side abstraction. Implemented in Infrastructure with Dapper + sp_Book_* reads.
// Application knows WHAT to read, never HOW (no SQL, no Dapper, no connection strings).
public interface IBookReader
{
    Task<IReadOnlyList<BookDto>> GetAllAsync(int userId, BookStatus? status = null, int? authorId = null, int? categoryId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<BookDto>> GetPagedAsync(
        int userId,
        BookStatus? status = null,
        int? authorId = null,
        int? categoryId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
    Task<BookDto?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookDto>> SearchAsync(int userId, string term, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookDto>> GetByAuthorAsync(int userId, int authorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookDto>> GetByCategoryAsync(int userId, int categoryId, CancellationToken cancellationToken = default);
}
