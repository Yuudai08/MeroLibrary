namespace BookLibrary.Application.DTOs;

public sealed record AuthResponseDto(
    int UserId,
    string Username,
    string Email,
    string Token);
