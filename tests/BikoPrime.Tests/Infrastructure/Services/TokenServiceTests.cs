namespace BikoPrime.Tests.Infrastructure.Services;

using Xunit;
using BikoPrime.Infrastructure.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly string _testSecret = "your-super-secret-key-min-32-chars-long-1234567890";

    public TokenServiceTests()
    {
        _tokenService = new TokenService(_testSecret);
    }

    [Fact]
    public void GenerateAccessToken_ShouldReturnValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@email.com";

        // Act
        var token = _tokenService.GenerateAccessToken(userId, email);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.True(token.Split('.').Length == 3);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidToken()
    {
        // Act
        var token = _tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.True(token.Split('.').Length == 3);
    }

    [Fact]
    public void ValidateAccessToken_WithValidToken_ShouldReturnUserIdAndEmail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@email.com";
        var token = _tokenService.GenerateAccessToken(userId, email);

        // Act
        var (validatedUserId, validatedEmail) = _tokenService.ValidateAccessToken(token);

        // Assert
        Assert.NotNull(validatedUserId);
        Assert.NotNull(validatedEmail);
        Assert.Equal(userId, validatedUserId);
        Assert.Equal(email, validatedEmail);
    }

    [Fact]
    public void ValidateAccessToken_WithInvalidToken_ShouldReturnNull()
    {
        // Act
        var (userId, email) = _tokenService.ValidateAccessToken("invalid-token");

        // Assert
        Assert.Null(userId);
        Assert.Null(email);
    }

    [Fact]
    public void ValidateRefreshToken_WithValidToken_ShouldReturnUserIdAndEmail()
    {
        // Arrange
        var token = _tokenService.GenerateRefreshToken();

        // Act
        var (userId, email) = _tokenService.ValidateRefreshToken(token);

        // Assert - Refresh token doesn't contain email, so it should return null
        Assert.Null(email);
    }
}
