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
            FirstName = "Test",
            LastName = "User",
            DisplayName = "testuser",
            UserName = "testuser",
            Email = "test@email.com",
            PhoneNumber = "(11) 99999-1111",
            Gender = "male",
            Pronoun = "he/him",
            DateOfBirth = new DateTime(1990, 1, 1),
            CEP = "12345-678",
            Password = "Password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new RegisterCommandHandler(_userManagerMock.Object, _tokenServiceMock.Object, _refreshTokenRepositoryMock.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.User);
        Assert.Equal("Conta criada com sucesso.", result.Message);
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowDomainException()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            DisplayName = "testuser",
            UserName = "testuser",
            Email = "existing@email.com",
            PhoneNumber = "(11) 99999-1111",
            Gender = "male",
            Pronoun = "he/him",
            DateOfBirth = new DateTime(1990, 1, 1),
            CEP = "12345-678",
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
            FirstName = "Test",
            LastName = "User",
            DisplayName = "testuser",
            UserName = "existinguser",
            Email = "new@email.com",
            PhoneNumber = "(11) 99999-1111",
            Gender = "male",
            Pronoun = "he/him",
            DateOfBirth = new DateTime(1990, 1, 1),
            CEP = "12345-678",
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
