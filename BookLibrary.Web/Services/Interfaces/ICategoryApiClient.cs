using BookLibrary.Web.Models;

namespace BookLibrary.Web.Services.Interfaces;

public interface ICategoryApiClient
{
    Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookDto>> GetBooksByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateCategoryAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
}
