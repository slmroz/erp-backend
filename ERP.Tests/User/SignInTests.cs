using ERP.Services.Abstractions.Security;
using ERP.Services.User.Commands.Handlers;
using ERP.Services.User.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace ERP.Tests.User;
// SignInTests.cs - POPRAWIONA wersja z JwtDto
public class SignInTests
{
    [Fact]
    public async Task SignIn_ShouldCreateTokenAndStore_WhenValidCredentials()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var authenticator = new Mock<IAuthenticator>();
        var passwordManager = new Mock<IPasswordManager>();
        var tokenStorage = new Mock<ITokenStorage>();

        var user = new Model.Model.User
        {
            Id = 1,
            Email = "john@test.com",
            Password = "hashedpass123",
            Role = (int)Model.Enum.Role.User
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expectedJwtDto = new JwtDto { AccessToken = "jwt eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." };
        authenticator.Setup(a => a.CreateToken(1, "User", "john@test.com")).Returns(expectedJwtDto);
        passwordManager.Setup(pm => pm.Validate("correctpass", "hashedpass123")).Returns(true);

        var handler = new SignInHandler(context, authenticator.Object, passwordManager.Object, tokenStorage.Object);
        var command = new SignInCommand("john@test.com", "correctpass");

        // Act
        await handler.HandleAsync(command);

        // Assert
        passwordManager.Verify(pm => pm.Validate("correctpass", "hashedpass123"), Times.Once);
        authenticator.Verify(a => a.CreateToken(1, "User", "john@test.com"), Times.Once);
        tokenStorage.Verify(t => t.Set(expectedJwtDto), Times.Once);
    }

    [Fact]
    public async Task SignIn_ShouldThrowUserNotFound_WhenEmailDoesNotExist()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var authenticator = new Mock<IAuthenticator>();
        var passwordManager = new Mock<IPasswordManager>();
        var tokenStorage = new Mock<ITokenStorage>();

        var handler = new SignInHandler(context, authenticator.Object, passwordManager.Object, tokenStorage.Object);
        var command = new SignInCommand("nonexistent@test.com", "password");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Contain("User nonexistent@test.com not exists");

        tokenStorage.Verify(t => t.Set(It.IsAny<JwtDto>()), Times.Never);
    }

    [Fact]
    public async Task SignIn_ShouldThrowInvalidCredentials_WhenWrongPassword()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var authenticator = new Mock<IAuthenticator>();
        var passwordManager = new Mock<IPasswordManager>();
        var tokenStorage = new Mock<ITokenStorage>();

        var user = new Model.Model.User
        {
            Email = "user@test.com",
            Password = "correcthash"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Validate("wrongpass", "correcthash")).Returns(false);

        var handler = new SignInHandler(context, authenticator.Object, passwordManager.Object, tokenStorage.Object);
        var command = new SignInCommand("user@test.com", "wrongpass");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("Invalud credentials");

        tokenStorage.Verify(t => t.Set(It.IsAny<JwtDto>()), Times.Never);
    }

    [Fact]
    public async Task SignIn_ShouldCreateTokenWithCorrectRole()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var authenticator = new Mock<IAuthenticator>();
        var passwordManager = new Mock<IPasswordManager>();
        var tokenStorage = new Mock<ITokenStorage>();

        var user = new Model.Model.User
        {
            Id = 42,
            Email = "admin@test.com",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.Admin
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var expectedJwtDto = new JwtDto { AccessToken = "admin-jwt-token" };
        authenticator.Setup(a => a.CreateToken(42, "Admin", "admin@test.com")).Returns(expectedJwtDto);
        passwordManager.Setup(pm => pm.Validate(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var handler = new SignInHandler(context, authenticator.Object, passwordManager.Object, tokenStorage.Object);
        var command = new SignInCommand("admin@test.com", "correctpass");

        // Act
        await handler.HandleAsync(command);

        // Assert
        authenticator.Verify(a => a.CreateToken(42, "Admin", "admin@test.com"), Times.Once);
        tokenStorage.Verify(t => t.Set(expectedJwtDto), Times.Once);
    }

    [Fact]
    public async Task SignIn_ShouldStoreOnlyAccessToken_NotWholeJwtDto()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var authenticator = new Mock<IAuthenticator>();
        var passwordManager = new Mock<IPasswordManager>();
        var tokenStorage = new Mock<ITokenStorage>();

        var user = new Model.Model.User { Id = 1, Email = "test@test.com", Password = "hash" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var jwtDto = new JwtDto { AccessToken = "actual-access-token-only" };
        authenticator.Setup(a => a.CreateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(jwtDto);
        passwordManager.Setup(pm => pm.Validate(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var handler = new SignInHandler(context, authenticator.Object, passwordManager.Object, tokenStorage.Object);
        var command = new SignInCommand("test@test.com", "pass");

        // Act
        await handler.HandleAsync(command);

        // Assert
        tokenStorage.Verify(t => t.Set(jwtDto), Times.Once);
    }
}
