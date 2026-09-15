using BookLibrary.Application.DTOs;

namespace BookLibrary.Application.Interfaces;

public interface ICategoryReader
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
