using ERP.Model.Enum;
using ERP.Services.Abstractions.Security;
using ERP.Services.User.Commands;
using ERP.Services.User.Commands.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ERP.Tests.Users;

public class SignUpTests
{
    [Fact]
    public async Task SignUp_ShouldCreateUser_WhenEmailNotExists()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        passwordManager.Setup(pm => pm.Secure("mypassword123")).Returns("hashedpassword123");

        var handler = new SignUpHandler(context, passwordManager.Object, clock);
        var command = new SignUpCommand(
            Email: "newuser@test.com",
            Password: "mypassword123",
            FirstName: "John",
            LastName: "Doe", 
            Role: (int)Role.User);

        // Act
        await handler.HandleAsync(command);

        // Assert
        passwordManager.Verify(pm => pm.Secure("mypassword123"), Times.Once);

        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@test.com");
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be("newuser@test.com");
        createdUser.Password.Should().Be("hashedpassword123");
        createdUser.FirstName.Should().Be("John");
        createdUser.LastName.Should().Be("Doe");
        createdUser.CreatedAt.Should().NotBe(default(DateTime));
        createdUser.Role.Should().Be((int)Model.Enum.Role.User);
    }

    [Fact]
    public async Task SignUp_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        // Existing user
        var existingUser = new Model.Model.User { Email = "existing@test.com", Password = "P@$$" };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var handler = new SignUpHandler(context, passwordManager.Object, clock);
        var command = new SignUpCommand("existing@test.com", "password123", "John", "Doe", (int)Role.User);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Contain("User existing@test.com already exists");

        passwordManager.Verify(pm => pm.Secure(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SignUp_ShouldUseDefaultRole_WhenRoleNotProvided()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        passwordManager.Setup(pm => pm.Secure("pass123")).Returns("hash123");

        var handler = new SignUpHandler(context, passwordManager.Object, clock);
        var command = new SignUpCommand("user@test.com", "pass123", "Jane", "Smith", Role: null);

        // Act
        await handler.HandleAsync(command);

        // Assert
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "user@test.com");
        createdUser!.Role.Should().Be((int)Model.Enum.Role.User); // Default role
    }

    [Fact]
    public async Task SignUp_ShouldUseProvidedRole_WhenRoleSpecified()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        passwordManager.Setup(pm => pm.Secure("adminpass")).Returns("adminhash");

        var handler = new SignUpHandler(context, passwordManager.Object, clock);
        var command = new SignUpCommand(
            "admin@test.com",
            "adminpass",
            "Admin",
            "User",
            Role: (int)Model.Enum.Role.Admin);

        // Act
        await handler.HandleAsync(command);

        // Assert
        var createdUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@test.com");
        createdUser!.Role.Should().Be((int)Model.Enum.Role.Admin);
    }

    [Fact]
    public async Task SignUp_ShouldHashPasswordNotStorePlainText()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        var plainPassword = "MySecurePass123!";
        var expectedHash = "securelyHashedVersionOfMySecurePass123!";
        passwordManager.Setup(pm => pm.Secure(plainPassword)).Returns(expectedHash);

        var handler = new SignUpHandler(context, passwordManager.Object, clock);
        var command = new SignUpCommand(plainPassword, plainPassword, "Test", "User", (int)Role.User);

        // Act
        await handler.HandleAsync(command);

        // Assert - hasło jest hashowane, NIE plain text!
        var createdUser = await context.Users.FirstOrDefaultAsync();
        createdUser!.Password.Should().Be(expectedHash);
        createdUser.Password.Should().NotBe(plainPassword);
    }

    [Fact]
    public async Task SignUp_ShouldSetHashedPassword_NotPlainText()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        passwordManager.Setup(pm => pm.Secure("pass")).Returns("HASHED");

        var handler = new SignUpHandler(context, passwordManager.Object, clock);
        var command = new SignUpCommand("test@test.com", "pass", "Test", "User", (int)Role.User);

        // Act
        await handler.HandleAsync(command);

        // Assert
        var createdUser = await context.Users.FirstOrDefaultAsync();
        createdUser!.Password.Should().Be("HASHED");
    }
}
