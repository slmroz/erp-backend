using ERP.Model.Abstractions;
using ERP.Services.Abstractions.Security;
using ERP.Services.User.Commands;
using ERP.Services.User.Commands.Handlers;
using FluentAssertions;
using Moq;

namespace ERP.Tests.User;

public class ResetPasswordTests
{
    [Fact]
    public async Task ResetPassword_ShouldUpdatePassword_WhenValidToken()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        var token = "valid-token-123";
        var user = new Model.Model.User
        {
            Email = "user@test.com",
            Password = "P@$$",
            PasswordResetToken = token,
            PasswordResetExpires = clock.Current().AddHours(2) // Valid token
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Secure("newpass123")).Returns("newhashedpass");

        var handler = new ResetPasswordHandler(context, passwordManager.Object, clock);
        var command = new ResetPasswordCommand(token, "newpass123");

        // Act
        await handler.HandleAsync(command);

        // Assert
        passwordManager.Verify(pm => pm.Secure("newpass123"), Times.Once);

        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.Password.Should().Be("newhashedpass");
        updatedUser.PasswordResetToken.Should().BeNull();
        updatedUser.PasswordResetExpires.Should().BeNull();
        updatedUser.LastModifiedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ResetPassword_ShouldThrowInvalidToken_WhenTokenNotFound()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        var handler = new ResetPasswordHandler(context, passwordManager.Object, clock);
        var command = new ResetPasswordCommand("nonexistent-token", "newpass");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("Invalid or expired reset token");
    }

    [Fact]
    public async Task ResetPassword_ShouldThrowExpiredToken()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Current()).Returns(new DateTime(2026, 2, 16, 12, 0, 0));

        var passwordManager = new Mock<IPasswordManager>();
        var token = "expired-token";

        var user = new Model.Model.User
        {
            Email = "user@test.com",
            Password = "P@$$",
            PasswordResetToken = token,
            PasswordResetExpires = new DateTime(2026, 2, 16, 10, 0, 0) // Expired
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ResetPasswordHandler(context, passwordManager.Object, clock.Object);
        var command = new ResetPasswordCommand(token, "newpass");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => handler.HandleAsync(command));
        exception.Message.Should().Be("Invalid or expired reset token");
    }

    [Fact]
    public async Task ResetPassword_ShouldClearTokenFields_AfterSuccess()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        var token = "test-token";
        var user = new Model.Model.User
        {
            Email = "user@test.com",
            PasswordResetToken = token,
            PasswordResetExpires = clock.Current().AddHours(1),
            Password = "oldpass"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Secure(It.IsAny<string>())).Returns("newhash");

        var handler = new ResetPasswordHandler(context, passwordManager.Object, clock);
        var command = new ResetPasswordCommand(token, "newpassword");

        // Act
        await handler.HandleAsync(command);

        // Assert - token fields zostały wyczyszczone
        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.PasswordResetToken.Should().BeNull();
        updatedUser.PasswordResetExpires.Should().BeNull();
        updatedUser.Password.Should().NotBe("oldpass");
        updatedUser.LastModifiedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ResetPassword_ShouldWorkWithMultipleUsers_SameTokenDoesNotAffectOthers()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var passwordManager = new Mock<IPasswordManager>();

        var token1 = "token1";
        var token2 = "token2";

        var user1 = new Model.Model.User
        {
            Email = "user1@test.com",
            Password = "P@$$",
            PasswordResetToken = token1,
            PasswordResetExpires = clock.Current().AddHours(1)
        };
        var user2 = new Model.Model.User
        {
            Email = "user2@test.com",
            Password = "P@$$",
            PasswordResetToken = token2,
            PasswordResetExpires = clock.Current().AddHours(1)
        };

        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        passwordManager.Setup(pm => pm.Secure(It.IsAny<string>())).Returns("newhash");

        var handler = new ResetPasswordHandler(context, passwordManager.Object, clock);

        // Act - reset tylko dla user1
        await handler.HandleAsync(new ResetPasswordCommand(token1, "newpass1"));

        // Assert
        var updatedUser1 = await context.Users.FindAsync(user1.Id);
        var updatedUser2 = await context.Users.FindAsync(user2.Id);

        updatedUser1!.PasswordResetToken.Should().BeNull();  // Wyczyszczony
        updatedUser2!.PasswordResetToken.Should().Be(token2); // Niewyczyszczony
    }
}

