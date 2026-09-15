using BookLibrary.Domain;

namespace BookLibrary.Application.Interfaces;

public interface IUserReader
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
