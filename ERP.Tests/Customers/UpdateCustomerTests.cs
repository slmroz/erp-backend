using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Customers;
public class UpdateCustomerTests
{
    [Fact]
    public async Task UpdateCustomer_ShouldModifyFields()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var customer = new Model.Model.Customer
        {
            Name = "Old",
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        var handler = new UpdateCustomerHandler(context, clock);

        var command = new UpdateCustomerCommand(customer.Id, "New", null, null, null, null, null, null, null);

        // act
        await handler.HandleAsync(command);

        var updated = await context.Customers.FindAsync(customer.Id);

        updated!.Name.Should().Be("New");
        updated.LastModifiedAt.Should().NotBeNull();
    }
}
