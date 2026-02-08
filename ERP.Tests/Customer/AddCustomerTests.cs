using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using ERP.Services.Customer.Validators;
using FluentAssertions;

namespace ERP.Tests.Customer;
public class AddCustomerTests
{
    [Fact]
    public async Task AddCustomer_ShouldPersistCustomer()
    {
        // arrange
        using var context = TestDbContextFactory.Create();
        var handler = new AddCustomerHandler(context);

        var command = new AddCustomerCommand
        {
            Name = "TeamMate",
            TaxId = "123",
            City = "Warsaw"
        };

        // act
        await handler.HandleAsync(command);

        // assert
        var customer = await context.Customers.FindAsync(command.Id);

        customer.Should().NotBeNull();
        customer!.Name.Should().Be("TeamMate");
        customer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        customer.RemovedAt.Should().BeNull();
    }

    [Fact]
    public void AddCustomer_ShouldFailWhenNameIsEmpty()
    {
        var validator = new AddCustomerCommandValidator();

        var result = validator.Validate(new AddCustomerCommand
        {
            Name = ""
        });

        result.IsValid.Should().BeFalse();
    }
}