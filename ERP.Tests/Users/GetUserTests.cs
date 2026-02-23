using ERP.Services.User.Queries;
using ERP.Services.User.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Users;
public class GetUserTests
{
    [Fact]
    public async Task GetUser_ShouldReturnUserDto()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var user = new Model.Model.User
        {
            Email = "john.doe@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.User
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUserQuery { UserId = user.Id });

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be("john.doe@test.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Role.Should().Be((int)Model.Enum.Role.User);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNull_WhenUserNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetUserHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUserQuery { UserId = 999 });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUser_ShouldMapUserDtoCorrectly()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var user = new Model.Model.User
        {
            Id = 42,
            Email = "admin@company.com",
            FirstName = "Admin",
            LastName = "User",
            Password = "P@$$",
            Role = (int)Model.Enum.Role.Admin
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetUserQuery { UserId = user.Id });

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Email.Should().Be("admin@company.com");
        result.Role.Should().Be((int)Model.Enum.Role.Admin);
        result.FirstName.Should().Be("Admin");
        result.LastName.Should().Be("User");
    }
}
