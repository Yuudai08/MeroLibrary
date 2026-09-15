using BookLibrary.Application.DTOs;

namespace BookLibrary.Application.Interfaces;

public interface ICategoryWriter
{
    Task<int> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
