namespace BikoPrime.Tests.Features.Auth.Login;

using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.Features.Auth.Login;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;

    public LoginCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@email.com",
            Password = "Password123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@email.com",
            UserName = "testuser"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns("access_token");

        _tokenServiceMock.Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns("refresh_token");

        _refreshTokenRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((RefreshToken)null!);

        _refreshTokenRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var handler = new LoginCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.User);
        Assert.Equal("access_token", result.Token);
        Assert.Equal("refresh_token", result.RefreshToken);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ShouldThrowDomainException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "nonexistent@email.com",
            Password = "Password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        var handler = new LoginCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("INVALID_CREDENTIALS", exception.Code);
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldThrowDomainException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@email.com",
            Password = "WrongPassword"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@email.com",
            UserName = "testuser"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = new LoginCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("INVALID_CREDENTIALS", exception.Code);
    }
}
