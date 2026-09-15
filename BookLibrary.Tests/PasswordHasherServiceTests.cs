using BookLibrary.Infrastructure.Services;
using Xunit;

namespace BookLibrary.Tests;

public class PasswordHasherServiceTests
{
    private readonly PasswordHasherService _hasher = new();

    [Fact]
    public void HashPassword_ProducesVerifiableHash()
    {
        var password = "Password123!";
        var hash = _hasher.HashPassword(password);

        Assert.True(_hasher.VerifyPassword(password, hash));
        Assert.False(_hasher.VerifyPassword("WrongPassword", hash));
    }

    [Fact]
    public void VerifyPassword_WithSeededDemoHash_Succeeds()
    {
        var seedHash = "YI7nEbZtH8hAo9n5QNA0sw==:cZib5u4oaAGl9iQTjhrT0sx3mczqkk8ACsI9jePud2E=";
        Assert.True(_hasher.VerifyPassword("Password123!", seedHash));
    }
}
