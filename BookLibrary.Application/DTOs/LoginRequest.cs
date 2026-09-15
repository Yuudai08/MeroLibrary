namespace BookLibrary.Application.DTOs;

public sealed record LoginRequest(
    string Username,
    string Password);
