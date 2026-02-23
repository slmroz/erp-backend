using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Customers;
public class RemoveCustomerTests
{
    [Fact]
    public async Task RemoveCustomer_ShouldSetRemovedAt()
    {
        using var context = TestDbContextFactory.Create();

        var customer = new Model.Model.Customer
        {
            Name = "ToRemove",
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new RemoveCustomerHandler(context);

        // act
        await handler.HandleAsync(
            new RemoveCustomerCommand(customer.Id));

        var removed = await context.Customers.FindAsync(customer.Id);

        removed!.RemovedAt.Should().NotBeNull();
    }
}
