using BookLibrary.Application.DTOs;

namespace BookLibrary.Application.Interfaces;

public interface IUserWriter
{
    Task<int> CreateAsync(RegisterRequest request, string passwordHash, CancellationToken cancellationToken = default);
}
