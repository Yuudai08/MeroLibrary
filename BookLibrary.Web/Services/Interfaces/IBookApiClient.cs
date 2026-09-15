using BookLibrary.Web.Models;

namespace BookLibrary.Web.Services.Interfaces;

public interface IBookApiClient
{
    Task<PagedResult<BookDto>> GetBooksAsync(BookStatus? status = null, int? authorId = null, int? categoryId = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<BookDto?> GetBookByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookDto>> SearchBooksAsync(string term, CancellationToken cancellationToken = default);
    Task<BookDto?> CreateBookAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateBookAsync(int id, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookAsync(int id, CancellationToken cancellationToken = default);
}
