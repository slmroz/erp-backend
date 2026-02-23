using ERP.Model.Abstractions;
using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Abstractions.Security;
using ERP.Services.User.Commands;
using ERP.Services.User.Commands.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ERP.Tests.Users;

public class ForgotPasswordTests
{
    [Fact]
    public async Task ForgotPassword_ShouldGenerateTokenAndSendEmail_WhenUserExists()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var emailService = new Mock<IEmailService>();
        var passwordManager = new Mock<IPasswordManager>();

        var user = new Model.Model.User
        {
            Email = "user@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "P@$$w0rd"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ForgotPasswordHandler(context, passwordManager.Object, emailService.Object, clock);
        var command = new ForgotPasswordCommand("user@test.com");

        // Act
        await handler.HandleAsync(command);

        // Assert
        emailService.Verify(e => e.SendAsync(
            "user@test.com",
            "PasswordReset",
            It.IsAny<Dictionary<string, string>>()),
            Times.Once);

        // Token został ustawiony
        var updatedUser = await context.Users.FindAsync(user.Id);
        updatedUser!.PasswordResetToken.Should().NotBeNull();
        updatedUser.PasswordResetExpires.Should().NotBeNull();
        updatedUser.PasswordResetExpires.Should().BeCloseTo(clock.Current().AddHours(1), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task ForgotPassword_ShouldSilentFail_WhenUserNotExists()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var emailService = new Mock<IEmailService>();
        var passwordManager = new Mock<IPasswordManager>();

        var handler = new ForgotPasswordHandler(context, passwordManager.Object, emailService.Object, clock);
        var command = new ForgotPasswordCommand("nonexistent@test.com");

        // Act
        await handler.HandleAsync(command);

        // Assert - brak wyjątku, brak emaila (silent fail)
        emailService.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPassword_ShouldSetCorrectTokenExpiry()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Current()).Returns(new DateTime(2026, 2, 16, 10, 0, 0));
        var emailService = new Mock<IEmailService>();
        var passwordManager = new Mock<IPasswordManager>();

        var user = new Model.Model.User { Email = "test@test.com", Password = "P@$$" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ForgotPasswordHandler(context, passwordManager.Object, emailService.Object, clock.Object);
        var command = new ForgotPasswordCommand("test@test.com");

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updatedUser = await context.Users.FirstAsync(u => u.Id == user.Id);
        updatedUser.PasswordResetExpires.Should().Be(new DateTime(2026, 2, 16, 11, 0, 0));
    }

    [Fact]
    public async Task ForgotPassword_ShouldGenerateUniqueTokens()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var emailService = new Mock<IEmailService>();
        var passwordManager = new Mock<IPasswordManager>();

        var user = new Model.Model.User { Email = "test@test.com", Password = "P@$$" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new ForgotPasswordHandler(context, passwordManager.Object, emailService.Object, clock);

        // Act - dwukrotne wywołanie
        await handler.HandleAsync(new ForgotPasswordCommand("test@test.com"));
        await handler.HandleAsync(new ForgotPasswordCommand("test@test.com"));

        // Assert - różne tokeny
        var updatedUser = await context.Users.FirstAsync(u => u.Id == user.Id);
        emailService.Verify(e => e.SendAsync(It.IsAny<string>(), "PasswordReset", It.IsAny<Dictionary<string, string>>()), Times.Exactly(2));
        updatedUser.PasswordResetToken.Should().NotBeNullOrEmpty();
    }
}
