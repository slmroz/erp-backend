using ERP.Services.Abstractions.CommonServices;
using ERP.Services.Customer.Commands;
using ERP.Services.Customer.Commands.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ERP.Tests.Customers;
public class AddInquiryTests
{
    [Fact]
    public async Task AddInquiryLead_ShouldCreateCustomerContactLead()
    {
        var clock = new ERP.Infrastructure.Time.Clock();
        var emailMock = new Mock<IEmailService>();
        using var context = TestDbContextFactory.Create();

        var handler = new AddInquiryLeadHandler(context, clock, emailMock.Object);
        var command = new AddInquiryLeadCommand(
            "Hamulce Toyota",
            "Tarcze ścierają się po 5k km",
            "AutoSerwis XYZ", "1234567890",
            "Jan", "Kowalski", "jan@xyz.pl", "123456789");
        await handler.HandleAsync(command);
        var leadId = command.Id;

        // Assert - 3 encje utworzone
        var customer = await context.Customers.FirstAsync();
        var contact = await context.Contacts.FirstAsync();
        var lead = await context.Leads.FirstAsync();

        customer.Name.Should().Be("AutoSerwis XYZ");
        contact.Email.Should().Be("jan@xyz.pl");
        lead.Subject.Should().Be("Hamulce Toyota");

        emailMock.Verify(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()));
    }
}
