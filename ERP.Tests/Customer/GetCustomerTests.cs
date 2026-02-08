using ERP.Services.Customer.Queries;
using ERP.Services.Customer.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Customer;
public class GetCustomerTests
{
    [Fact]
    public async Task GetCustomer_ShouldReturnCustomer_WhenExists()
    {
        using var context = TestDbContextFactory.Create();

        var customer = new Model.Model.Customer
        {
            Name = "ACME",
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new GetCustomerHandler(context);

        // act
        var result = await handler.HandleAsync(
            new GetCustomerQuery() { Id = customer.Id });

        // assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ACME");
    }

    [Fact]
    public async Task GetCustomer_ShouldReturnNull_WhenRemoved()
    {
        using var context = TestDbContextFactory.Create();

        var customer = new Model.Model.Customer
        {
            Name = "Removed",
            CreatedAt = DateTime.UtcNow,
            RemovedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new GetCustomerHandler(context);

        var result = await handler.HandleAsync(
            new GetCustomerQuery() { Id = customer.Id });

        result.Should().BeNull();
    }
}
