using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Contacts;
public class RemoveContactTests
{
    [Fact]
    public async Task RemoveContact_ShouldSoftDelete()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var customer = new Model.Model.Customer { Name = "Test Corp", CreatedAt = DateTime.UtcNow };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var contact = new Model.Model.Contact
        {
            CustomerId = customer.Id,
            FirstName = "John",
            LastName = "Doe"
        };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new RemoveContactHandler(context, clock);

        // Act
        await handler.HandleAsync(new RemoveContactCommand(contact.Id));

        // Assert
        var removed = await context.Contacts.FindAsync(contact.Id);
        removed.Should().NotBeNull();
        removed!.RemovedAt.Should().NotBeNull();
        removed.RemovedAt.Should().BeCloseTo(clock.Current(), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task RemoveContact_ShouldThrow_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new RemoveContactHandler(context, clock);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(new RemoveContactCommand(999)));
    }
}

