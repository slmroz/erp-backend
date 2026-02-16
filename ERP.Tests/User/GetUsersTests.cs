using ERP.Services.User.Queries;
using ERP.Services.User.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.User;
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
        result.Should().HaveCount(2);
        result.Should().ContainSingle(u => u.Email == "user1@test.com");
        result.Should().ContainSingle(u => u.FirstName == "Jane");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnEmpty_WhenNoUsers()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetUsersHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUsersQuery());

        // Assert
        result.Should().BeEmpty();
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
        var dto = result.Single();
        dto.Email.Should().Be("test@test.com");
        dto.FirstName.Should().Be("Test");
        dto.LastName.Should().Be("User");
        dto.Role.Should().Be((int)Model.Enum.Role.User);
    }
}
