using BookLibrary.Application.DTOs;

namespace BookLibrary.Application.Interfaces;

public interface IAuthorWriter
{
    Task<int> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteWithBooksAsync(int authorId, CancellationToken cancellationToken = default);
}
