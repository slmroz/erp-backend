using ERP.Model.Enum;
using ERP.Model.Model;
using ERP.Services.User.Queries;
using ERP.Services.User.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Users;
public class GetUsersTests
{
    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var user1 = new Model.Model.User
        {
            Email = "user1@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.User
        };
        var user2 = new Model.Model.User
        {
            Email = "user2@test.com",
            FirstName = "Jane",
            LastName = "Smith",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.Admin
        };

        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        var handler = new GetUsersHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUsersQuery());

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(u => u.Email == "user1@test.com");
        result.Items.Should().ContainSingle(u => u.FirstName == "Jane");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmpty_WhenNoUsers()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetUsersHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUsersQuery());

        // Assert
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsers_ShouldMapUserDtoCorrectly()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var user = new Model.Model.User
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Password = "hashedpass",
            Role = (int)Model.Enum.Role.User,
            CreatedAt = DateTime.UtcNow
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUsersHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUsersQuery());

        // Assert
        var dto = result.Items.Single();
        dto.Email.Should().Be("test@test.com");
        dto.FirstName.Should().Be("Test");
        dto.LastName.Should().Be("User");
        dto.Role.Should().Be((int)Model.Enum.Role.User);
    }

    [Fact]
    public async Task GetUsers_ShouldFilterByRole()
    {
        using var context = TestDbContextFactory.Create();
        context.Users.AddRange(
            UserTestFactory.CreateUser("jan@admin.com", "Jan", "Kowalski", (int)Role.Admin),
            UserTestFactory.CreateUser("anna@manager.com", "Anna", "Nowak", (int)Role.Manager),
            UserTestFactory.CreateUser("piotr@user.com", "Piotr", "Wiśniewski", (int)Role.Admin)
        );
        await context.SaveChangesAsync();

        var handler = new GetUsersHandler(context);
        var result = await handler.HandleAsync(new GetUsersQuery(Role: (int)Role.Admin));

        result.Items.Should().HaveCount(2);
        result.Items.All(u => u.Role == (int)Role.Admin).Should().BeTrue();
    }

    [Fact]
    public async Task GetUsers_ShouldSearchCaseInsensitive()
    {
        using var context = TestDbContextFactory.Create();
        context.Users.Add(
            UserTestFactory.CreateUser("JAN@TEST.COM", "JAN", "KOWALSKI", (int)Role.Admin));
        await context.SaveChangesAsync();

        var handler = new GetUsersHandler(context);
        var result = await handler.HandleAsync(new GetUsersQuery(Search: "jan"));

        result.Items.Should().HaveCount(1);
        result.Items[0].Email.Should().Be("JAN@TEST.COM");
    }

}

public static class UserTestFactory
{
    public static User CreateUser(string email, string firstName, string lastName, int role)
    {
        return new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            LastUpdatedAt = DateTime.UtcNow.AddDays(-1),
            Password = "TestPassword"
        };
    }
}