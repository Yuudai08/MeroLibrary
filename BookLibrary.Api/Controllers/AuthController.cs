using System.Text.RegularExpressions;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserReader _userReader;
    private readonly IUserWriter _userWriter;
    private readonly IPasswordHasherService _hasher;
    private readonly ITokenService _tokenService;

    public AuthController(
        IUserReader userReader,
        IUserWriter userWriter,
        IPasswordHasherService hasher,
        ITokenService tokenService)
    {
        _userReader = userReader;
        _userWriter = userWriter;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || request.Username.Trim().Length < 3)
        {
            return BadRequest(new { message = "Username must be at least 3 characters long." });
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !Regex.IsMatch(request.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return BadRequest(new { message = "A valid email address is required." });
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            return BadRequest(new { message = "Password must be at least 6 characters long." });
        }

        var existingUser = await _userReader.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (existingUser != null)
        {
            return Conflict(new { message = "Username is already taken." });
        }

        var passwordHash = _hasher.HashPassword(request.Password);
        var newUserId = await _userWriter.CreateAsync(request, passwordHash, cancellationToken);

        var token = _tokenService.GenerateToken(newUserId, request.Username.Trim(), request.Email.Trim());
        var response = new AuthResponseDto(newUserId, request.Username.Trim(), request.Email.Trim(), token);

        return CreatedAtAction(nameof(Login), new { id = newUserId }, response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username and password are required." });
        }

        var user = await _userReader.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (user == null || !_hasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email);
        var response = new AuthResponseDto(user.Id, user.Username, user.Email, token);

        return Ok(response);
    }
}
