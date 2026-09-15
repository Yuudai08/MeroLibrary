namespace BookLibrary.Application.DTOs;

public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password);
