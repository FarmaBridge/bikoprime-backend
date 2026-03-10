namespace BikoPrime.Tests.Features.Auth.Login;

using Xunit;
using FluentValidation.TestHelper;
using BikoPrime.Application.Features.Auth.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@email.com",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyEmail_ShouldHaveError()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = string.Empty,
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "invalid-email",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithEmptyPassword_ShouldHaveError()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@email.com",
            Password = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
