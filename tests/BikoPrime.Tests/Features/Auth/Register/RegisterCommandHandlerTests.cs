namespace BikoPrime.Tests.Features.Auth.Register;

using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.Features.Auth.Register;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class RegisterCommandHandlerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;

    public RegisterCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldRegisterUserAndReturnAuthResponse()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "testuser",
            Email = "test@email.com",
            Phone = "(11) 99999-1111",
            Password = "Password123",
            AvatarUrl = null,
            Location = null,
            Bio = null
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns("access_token");

        _tokenServiceMock.Setup(x => x.GenerateRefreshToken(It.IsAny<Guid>(), It.IsAny<string>()))
            .Returns("refresh_token");

        _refreshTokenRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        var handler = new RegisterCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.User);
        Assert.Equal("access_token", result.Token);
        Assert.Equal("refresh_token", result.RefreshToken);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowDomainException()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "testuser",
            Email = "existing@email.com",
            Phone = "(11) 99999-1111",
            Password = "Password123"
        };

        var existingUser = new User { Id = Guid.NewGuid(), Email = "existing@email.com" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        var handler = new RegisterCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("EMAIL_IN_USE", exception.Code);
    }

    [Fact]
    public async Task Handle_WithExistingUsername_ShouldThrowDomainException()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "existinguser",
            Email = "new@email.com",
            Phone = "(11) 99999-1111",
            Password = "Password123"
        };

        var existingUser = new User { Id = Guid.NewGuid(), UserName = "existinguser" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(existingUser);

        var handler = new RegisterCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Equal("USERNAME_IN_USE", exception.Code);
    }
}
