namespace BikoPrime.Tests.Features.Auth.Register;

using Xunit;
using FluentValidation.TestHelper;
using BikoPrime.Application.Features.Auth.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator;

    public RegisterCommandValidatorTests()
    {
        _validator = new RegisterCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "testuser",
            Email = "test@email.com",
            Phone = "(11) 99999-1111",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldHaveError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = string.Empty,
            UserName = "testuser",
            Email = "test@email.com",
            Phone = "(11) 99999-1111",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "testuser",
            Email = "invalid-email",
            Phone = "(11) 99999-1111",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithShortPassword_ShouldHaveError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "testuser",
            Email = "test@email.com",
            Phone = "(11) 99999-1111",
            Password = "short"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WithInvalidUsername_ShouldHaveError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Name = "Test User",
            UserName = "invalid@user",
            Email = "test@email.com",
            Phone = "(11) 99999-1111",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }
}
