using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Contact;
public class AddContactTests
{
    [Fact]
    public async Task AddContact_ShouldCreateContact()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange - dodaj Customer dla testu
        var customer = new Model.Model.Customer { Name = "Test Corp", CreatedAt = DateTime.UtcNow };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new AddContactHandler(context, clock);
        var command = new AddContactCommand() { CustomerId = customer.Id, FirstName = "John", LastName = "Doe", PhoneNo = "+123456789", Email = "john@test.com" };

        // Act
        await handler.HandleAsync(command);
        var contactId = command.Id;

        // Assert
        var contact = await context.Contacts.FindAsync(contactId);
        contact.Should().NotBeNull();
        contact!.FirstName.Should().Be("John");
        contact.LastName.Should().Be("Doe");
        contact.Email.Should().Be("john@test.com");
        contact.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task AddContact_ShouldThrow_WhenCustomerNotExists()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new AddContactHandler(context, clock);
        var command = new AddContactCommand() { CustomerId = 999, FirstName = "John", LastName = "Doe", PhoneNo = null, Email = "john@test.com" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(command));
    }
}
