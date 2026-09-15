namespace BookLibrary.Web.Models;

public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password);
