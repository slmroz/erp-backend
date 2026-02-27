using ERP.Model.Model;
using ERP.Services.Customer.Queries;
using ERP.Services.Customer.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Customers;
public class GetCustomersTests
{
    [Fact]
    public async Task GetCustomers_ShouldReturnOnlyNotRemoved()
    {
        using var context = TestDbContextFactory.Create();

        context.Customers.AddRange(
            new Model.Model.Customer { Name = "A", CreatedAt = DateTime.UtcNow },
            new Model.Model.Customer { Name = "B", CreatedAt = DateTime.UtcNow, RemovedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();

        var handler = new GetCustomersHandler(context);

        var result = await handler.HandleAsync(
            new GetCustomersQuery(Page: 1, PageSize: 10));

        result.Items.Should().HaveCount(1);
        result.Items.First().Name.Should().Be("A");
    }

    [Fact]
    public async Task GetCustomers_ShouldApplyPaging()
    {
        using var context = TestDbContextFactory.Create();

        for (int i = 1; i <= 20; i++)
        {
            context.Customers.Add(new Model.Model.Customer
            {
                Name = $"C{i}",
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();

        var handler = new GetCustomersHandler(context);

        var result = await handler.HandleAsync(
            new GetCustomersQuery(Page: 2, PageSize: 5));

        result.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetCustomers_ShouldSortByNameAsc()
    {
        using var context = TestDbContextFactory.Create();
        context.Customers.AddRange(
            new Customer { Name = "Zebra Corp", TaxId = "123", CreatedAt = DateTime.UtcNow },
            new Customer { Name = "Apple Inc", TaxId = "456", CreatedAt = DateTime.UtcNow },
            new Customer { Name = "banana LLC", TaxId = "789", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetCustomersHandler(context);
        var result = await handler.HandleAsync(new GetCustomersQuery(SortBy: "name", SortOrder: "asc"));

        result.Items[0].Name.Should().Be("Apple Inc");    // A przed B przed Z
        result.Items[1].Name.Should().Be("banana LLC");
        result.Items[2].Name.Should().Be("Zebra Corp");
    }

    [Fact]
    public async Task GetCustomers_ShouldSortCaseInsensitive()
    {
        using var context = TestDbContextFactory.Create();
        context.Customers.AddRange(
            new Customer { Name = "apple", CreatedAt = DateTime.UtcNow },
            new Customer { Name = "Banana", CreatedAt = DateTime.UtcNow },
            new Customer { Name = "APPLE INC", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetCustomersHandler(context);
        var result = await handler.HandleAsync(new GetCustomersQuery(SortBy: "name"));

        result.Items[0].Name.Should().Be("apple");        // a, A, B
        result.Items[1].Name.Should().Be("APPLE INC");
        result.Items[2].Name.Should().Be("Banana");
    }

}
