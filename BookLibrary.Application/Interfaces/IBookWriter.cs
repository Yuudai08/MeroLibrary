using BookLibrary.Application.DTOs;

namespace BookLibrary.Application.Interfaces;

// Command side abstraction. Implemented in Infrastructure with EF Core + sp_Book_* writes.
// bool return = rows-affected signal: false means Id not found (API maps to 404).
public interface IBookWriter
{
    Task<int> CreateAsync(int userId, CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, int userId, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
}
