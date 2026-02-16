using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Contact;
public class UpdateContactTests
{
    [Fact]
    public async Task UpdateContact_ShouldModifyFields()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var customer = new Model.Model.Customer { Name = "Test Corp", CreatedAt = DateTime.UtcNow };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var contact = new Model.Model.Contact
        {
            CustomerId = customer.Id,
            FirstName = "Old",
            LastName = "Name",
            PhoneNo = "+123"
        };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdateContactHandler(context, clock);
        var command = new UpdateContactCommand(contact.Id, customer.Id, "New", "Name", "+999", "new@test.com");

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updated = await context.Contacts.FindAsync(contact.Id);
        updated!.FirstName.Should().Be("New");
        updated.LastName.Should().Be("Name");
        updated.PhoneNo.Should().Be("+999");
        updated.Email.Should().Be("new@test.com");
        updated.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task UpdateContact_ShouldThrow_WhenContactNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdateContactHandler(context, clock);
        var command = new UpdateContactCommand(999, 1, "John", "Doe", null, "test@test.com");

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(command));
    }

    [Fact]
    public async Task UpdateContact_ShouldThrow_WhenNewCustomerNotExists()
    {
        using var context = TestDbContextFactory.Create();

        var customer = new Model.Model.Customer { Name = "Test Corp", CreatedAt = DateTime.UtcNow };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var contact = new Model.Model.Contact { CustomerId = customer.Id, FirstName = "John", LastName = "Doe" };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdateContactHandler(context, clock);
        // Invalid CustomerId = 999
        var command = new UpdateContactCommand(contact.Id, 999, "Updated", "Name", null, null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(command));
    }
}
