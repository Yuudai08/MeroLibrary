namespace BookLibrary.Application.DTOs;

public sealed record UserDto(
    int Id,
    string Username,
    string Email,
    DateTime CreatedAt);
