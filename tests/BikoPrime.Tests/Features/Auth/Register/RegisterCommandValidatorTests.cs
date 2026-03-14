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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyFirstName_ShouldHaveError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = string.Empty,
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

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveError()
    {
        // Arrange
        var command = new RegisterCommand
        {
            FirstName = "Test",
            LastName = "User",
            DisplayName = "testuser",
            UserName = "testuser",
            Email = "invalid-email",
            PhoneNumber = "(11) 99999-1111",
            Gender = "male",
            Pronoun = "he/him",
            DateOfBirth = new DateTime(1990, 1, 1),
            CEP = "12345-678",
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
            FirstName = "Test",
            LastName = "User",
            DisplayName = "testuser",
            UserName = "invalid@user",
            Email = "test@email.com",
            PhoneNumber = "(11) 99999-1111",
            Gender = "male",
            Pronoun = "he/him",
            DateOfBirth = new DateTime(1990, 1, 1),
            CEP = "12345-678",
            Password = "Password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }
}
