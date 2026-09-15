using BookLibrary.Web.Models;

namespace BookLibrary.Web.Services.Interfaces;

public interface IAuthApiClient
{
    Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
