using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Contact;
public class GetContactTests
{
    [Fact]
    public async Task GetContact_ShouldReturnDto()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var customer = new Model.Model.Customer { Name = "Microsoft", CreatedAt = DateTime.UtcNow };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var contact = new Model.Model.Contact
        {
            CustomerId = customer.Id,
            FirstName = "Satya",
            LastName = "Nadella",
            Email = "satya@microsoft.com"
        };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var handler = new GetContactHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetContactQuery(contact.Id));

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contact.Id);
        result.FirstName.Should().Be("Satya");
        result.CustomerName.Should().Be("Microsoft");
    }

    [Fact]
    public async Task GetContact_ShouldReturnNull_WhenRemoved()
    {
        using var context = TestDbContextFactory.Create();

        var contact = new Model.Model.Contact
        {
            FirstName = "John",
            LastName = "Doe",
            RemovedAt = DateTime.UtcNow
        };
        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        var handler = new GetContactHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetContactQuery(contact.Id));

        // Assert
        result.Should().BeNull();
    }
}
