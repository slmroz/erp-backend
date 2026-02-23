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
}
