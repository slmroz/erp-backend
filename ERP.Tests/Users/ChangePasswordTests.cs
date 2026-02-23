using ERP.Services.Abstractions.Security;
using ERP.Services.User.Commands;
using ERP.Services.User.Commands.Handlers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace ERP.Tests.Users;

public class ChangePasswordTests
{
    [Fact]
    public async Task ChangePassword_ShouldUpdatePassword_WhenValid()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();
        var httpContextAccessor = CreateHttpContextAccessor("1"); // userId = 1

        // Setup test user
        var user = new Model.Model.User
        {
            Id = 1,
            Email = "test@test.com",
            Password = "oldhashedpass",
            FirstName = "Test",
            LastName = "User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Validate("currentpass", "oldhashedpass")).Returns(true);
        passwordManager.Setup(pm => pm.Secure("newpass123")).Returns("newhashedpass");

        var handler = new ChangePasswordHandler(context, passwordManager.Object, clock, httpContextAccessor.Object);
        var command = new ChangePasswordCommand("currentpass", "newpass123");

        // Act
        await handler.HandleAsync(command);

        // Assert
        passwordManager.Verify(pm => pm.Validate("currentpass", "oldhashedpass"), Times.Once);
        passwordManager.Verify(pm => pm.Secure("newpass123"), Times.Once);

        var updatedUser = await context.Users.FindAsync(1);
        updatedUser!.Password.Should().Be("newhashedpass");
        updatedUser.LastModifiedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ChangePassword_ShouldThrowUnauthorized_WhenNoUserInContext()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();
        var httpContextAccessor = CreateHttpContextAccessor(null); // No userId

        var handler = new ChangePasswordHandler(context, passwordManager.Object, clock, httpContextAccessor.Object);
        var command = new ChangePasswordCommand("old", "new");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("User not authenticated");
    }

    [Fact]
    public async Task ChangePassword_ShouldThrowUserNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();
        var httpContextAccessor = CreateHttpContextAccessor("999"); // Non-existent user

        var handler = new ChangePasswordHandler(context, passwordManager.Object, clock, httpContextAccessor.Object);
        var command = new ChangePasswordCommand("old", "new");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ChangePassword_ShouldThrowInvalidCurrentPassword()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();
        var httpContextAccessor = CreateHttpContextAccessor("1");

        var user = new Model.Model.User
        {
            Id = 1,
            Password = "correcthash",
            Email = "user@domain.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Validate("wrongpass", "correcthash")).Returns(false);

        var handler = new ChangePasswordHandler(context, passwordManager.Object, clock, httpContextAccessor.Object);
        var command = new ChangePasswordCommand("wrongpass", "newpass");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("Current password is incorrect");
    }

    [Fact]
    public async Task ChangePassword_ShouldThrowSamePassword()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();
        var httpContextAccessor = CreateHttpContextAccessor("1");

        var user = new Model.Model.User
        {
            Id = 1,
            Password = "currenthash",
            Email = "user@domain.com"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Validate("currentpass", "currenthash")).Returns(true);
        passwordManager.Setup(pm => pm.Secure("currentpass")).Returns("currenthash"); // Same hash

        var handler = new ChangePasswordHandler(context, passwordManager.Object, clock, httpContextAccessor.Object);
        var command = new ChangePasswordCommand("currentpass", "currentpass");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("New password cannot be the same as current");
    }

    // Helper method
    private Mock<IHttpContextAccessor> CreateHttpContextAccessor(string? userId)
    {
        var claimsPrincipal = userId != null
            ? new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "mock"))
            : new ClaimsPrincipal();

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        return httpContextAccessor;
    }
}