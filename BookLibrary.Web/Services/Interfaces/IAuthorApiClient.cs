using BookLibrary.Web.Models;

namespace BookLibrary.Web.Services.Interfaces;

public interface IAuthorApiClient
{
    Task<IReadOnlyList<AuthorDto>> GetAllAuthorsAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto?> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookDto>> GetBooksByAuthorAsync(int authorId, CancellationToken cancellationToken = default);
    Task<AuthorDto?> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAuthorAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAuthorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAuthorWithBooksAsync(int id, CancellationToken cancellationToken = default);
}
