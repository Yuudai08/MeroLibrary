namespace BookLibrary.Web.Models;

public sealed record LoginRequest(
    string Username,
    string Password);
