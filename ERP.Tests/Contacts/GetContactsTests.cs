using ERP.Model.Model;
using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Contacts;
public class GetContactsTests
{
    [Fact]
    public async Task GetContacts_ShouldReturnPagedResults()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange - dodaj dane testowe
        var customer1 = new Model.Model.Customer { Name = "Microsoft" };
        var customer2 = new Model.Model.Customer { Name = "Apple" };
        context.Customers.AddRange(customer1, customer2);
        await context.SaveChangesAsync();

        var contacts = new[]
        {
            new Model.Model.Contact { CustomerId = customer1.Id, FirstName = "John", LastName = "A" },
            new Model.Model.Contact { CustomerId = customer1.Id, FirstName = "Jane", LastName = "B" },
            new Model.Model.Contact { CustomerId = customer2.Id, FirstName = "Bob", LastName = "C" }
        };
        context.Contacts.AddRange(contacts);
        await context.SaveChangesAsync();

        var handler = new GetContactsHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetContactsQuery(1, 2));

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.Items[0].LastName.Should().Be("A");
        result.Items[1].LastName.Should().Be("B");
    }

    [Fact]
    public async Task GetContacts_ShouldFilterByCustomerId()
    {
        using var context = TestDbContextFactory.Create();

        var customer1 = new Model.Model.Customer { Name = "Microsoft" };
        var customer2 = new Model.Model.Customer { Name = "Apple" };
        context.Customers.AddRange(customer1, customer2);
        await context.SaveChangesAsync();

        var contact1 = new Model.Model.Contact { CustomerId = customer1.Id, FirstName = "John" };
        var contact2 = new Model.Model.Contact { CustomerId = customer2.Id, FirstName = "Jane" };
        context.Contacts.AddRange(contact1, contact2);
        await context.SaveChangesAsync();

        var handler = new GetContactsHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetContactsQuery(CustomerId: customer1.Id));

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].CustomerId.Should().Be(customer1.Id);
    }

    [Fact]
    public async Task GetContacts_ShouldSearchCaseInsensitive()
    {
        using var context = TestDbContextFactory.Create();
        context.Customers.Add(new Customer { Id = 1, Name = "Acme Corp" });
        context.Contacts.AddRange(
            new Contact { FirstName = "JAN", LastName = "Kowalski", Email = "jan@test.com", CustomerId = 1, CreatedAt = DateTime.UtcNow },
            new Contact { FirstName = "Anna", LastName = "Nowak", Email = "anna@test.com", CustomerId = 1, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetContactsHandler(context);
        var result = await handler.HandleAsync(new GetContactsQuery(Search: "jan"));

        result.Items.Should().HaveCount(1);
        result.Items[0].FirstName.Should().Be("JAN");  // Znajdzie mimo wielkości liter
    }

    [Fact]
    public async Task GetContacts_ShouldSortByLastNameDesc()
    {
        using var context = TestDbContextFactory.Create();
        context.Contacts.AddRange(
            new Contact { FirstName = "Jan", LastName = "Nowak", CreatedAt = DateTime.UtcNow },
            new Contact { FirstName = "Anna", LastName = "Zając", CreatedAt = DateTime.UtcNow },
            new Contact { FirstName = "Piotr", LastName = "Kowalski", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetContactsHandler(context);
        var result = await handler.HandleAsync(new GetContactsQuery(SortBy: "lastname", SortOrder: "desc"));

        result.Items[0].LastName.Should().Be("Zając");     // Z > N > K
        result.Items[1].LastName.Should().Be("Nowak");
        result.Items[2].LastName.Should().Be("Kowalski");
    }

}

