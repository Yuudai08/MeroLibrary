using BookLibrary.Api.Controllers;
using BookLibrary.Application.DTOs;
using BookLibrary.Application.Interfaces;
using BookLibrary.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookLibrary.Tests;

public class AuthControllerTests
{
    private readonly Mock<IUserReader> _mockUserReader;
    private readonly Mock<IUserWriter> _mockUserWriter;
    private readonly Mock<IPasswordHasherService> _mockHasher;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockUserReader = new Mock<IUserReader>();
        _mockUserWriter = new Mock<IUserWriter>();
        _mockHasher = new Mock<IPasswordHasherService>();
        _mockTokenService = new Mock<ITokenService>();

        _controller = new AuthController(
            _mockUserReader.Object,
            _mockUserWriter.Object,
            _mockHasher.Object,
            _mockTokenService.Object);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsCreatedWithToken()
    {
        // Arrange
        var request = new RegisterRequest("newuser", "newuser@example.com", "Secret123!");
        _mockUserReader.Setup(r => r.GetByUsernameAsync("newuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _mockHasher.Setup(h => h.HashPassword("Secret123!"))
            .Returns("hashed_pwd");
        _mockUserWriter.Setup(w => w.CreateAsync(request, "hashed_pwd", It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _mockTokenService.Setup(t => t.GenerateToken(10, "newuser", "newuser@example.com"))
            .Returns("mock_jwt_token");

        // Act
        var result = await _controller.Register(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(createdResult.Value);
        Assert.Equal(10, response.UserId);
        Assert.Equal("newuser", response.Username);
        Assert.Equal("mock_jwt_token", response.Token);
    }

    [Fact]
    public async Task Register_DuplicateUsername_ReturnsConflict()
    {
        // Arrange
        var request = new RegisterRequest("existinguser", "user@example.com", "Secret123!");
        _mockUserReader.Setup(r => r.GetByUsernameAsync("existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = 1, Username = "existinguser" });

        // Act
        var result = await _controller.Register(request, CancellationToken.None);

        // Assert
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var user = new User { Id = 5, Username = "alice", Email = "alice@example.com", PasswordHash = "hashed_val" };
        _mockUserReader.Setup(r => r.GetByUsernameAsync("alice", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockHasher.Setup(h => h.VerifyPassword("Alice123!", "hashed_val"))
            .Returns(true);
        _mockTokenService.Setup(t => t.GenerateToken(5, "alice", "alice@example.com"))
            .Returns("alice_jwt_token");

        // Act
        var result = await _controller.Login(new LoginRequest("alice", "Alice123!"), CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponseDto>(okResult.Value);
        Assert.Equal("alice_jwt_token", response.Token);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var user = new User { Id = 5, Username = "alice", Email = "alice@example.com", PasswordHash = "hashed_val" };
        _mockUserReader.Setup(r => r.GetByUsernameAsync("alice", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockHasher.Setup(h => h.VerifyPassword("WrongPassword", "hashed_val"))
            .Returns(false);

        // Act
        var result = await _controller.Login(new LoginRequest("alice", "WrongPassword"), CancellationToken.None);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }
}
