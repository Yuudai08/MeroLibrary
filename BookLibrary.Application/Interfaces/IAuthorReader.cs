using BookLibrary.Application.DTOs;

namespace BookLibrary.Application.Interfaces;

public interface IAuthorReader
{
    Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
