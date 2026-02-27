using ERP.Model.Abstractions;
using ERP.Services.User.Commands;
using ERP.Services.User.Commands.Handlers;
using FluentAssertions;
using Moq;

namespace ERP.Tests.Users;

public class UpdateUserTests
{
    [Fact]
    public async Task UpdateUser_ShouldModifyFields()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        // Arrange
        var user = new Model.Model.User
        {
            Id = 1,
            Email = "old@test.com",
            FirstName = "Old",
            LastName = "User",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.User,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new UpdateUserHandler(context, clock);
        var command = new UpdateUserCommand(
            Id: 1,
            Email: "new@test.com",
            FirstName: "New",
            LastName: "User",
            Role: (int)Model.Enum.Role.Admin);

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updated = await context.Users.FindAsync(1);
        updated!.Email.Should().Be("new@test.com");
        updated.FirstName.Should().Be("New");
        updated.LastName.Should().Be("User");
        updated.Role.Should().Be((int)Model.Enum.Role.Admin);
        updated.LastUpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateUser_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdateUserHandler(context, clock);
        var command = new UpdateUserCommand(999, (int)Model.Enum.Role.User, "test@test.com", "Test", "User");

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task UpdateUser_ShouldThrowNotFound_WhenUserRemoved()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        // Arrange - removed user
        var user = new Model.Model.User
        {
            Id = 1,
            Email = "removed@test.com",
            Password = "P@$$",
            RemovedAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new UpdateUserHandler(context, clock);
        var command = new UpdateUserCommand(1, (int)Model.Enum.Role.User, "new@test.com", "New", "Name");

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateRole()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        // Arrange
        var user = new Model.Model.User
        {
            Id = 42,
            Email = "admin-candidate@test.com",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.User
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new UpdateUserHandler(context, clock);
        var command = new UpdateUserCommand(
            42,
            (int)Model.Enum.Role.Admin,
            "admin@test.com",
            "Admin",
            "User");

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updated = await context.Users.FindAsync(42);
        updated!.Role.Should().Be((int)Model.Enum.Role.Admin);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateLastModifiedAt()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Current()).Returns(new DateTime(2026, 2, 16, 15, 30, 0));

        // Arrange
        var user = new Model.Model.User
        {
            Id = 1,
            Email = "test@test.com",
            Password = "P@$$",
            LastUpdatedAt = new DateTime(2026, 2, 16, 10, 0, 0) // Old timestamp
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new UpdateUserHandler(context, clock.Object);
        var command = new UpdateUserCommand(1, (int)Model.Enum.Role.User, "updated@test.com", "Updated", "User");

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updated = await context.Users.FindAsync(1);
        updated!.LastUpdatedAt.Should().Be(new DateTime(2026, 2, 16, 15, 30, 0));
    }
}

