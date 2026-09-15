namespace BookLibrary.Web.Models;

public sealed record AuthResponseDto(
    int UserId,
    string Username,
    string Email,
    string Token);
