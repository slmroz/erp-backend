using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Currencies;
public class UpdateCurrencyTests
{
    [Fact]
    public async Task UpdateCurrency_ShouldModifyRate()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var currency = new Currency { Id = 1, BaseCurrency = "USD", TargetCurrency = "PLN", Rate = 3.5m };
        context.Currencies.Add(currency);
        await context.SaveChangesAsync();

        var handler = new UpdateCurrencyHandler(context, clock);
        await handler.HandleAsync(new UpdateCurrencyCommand(1, "USD", "PLN", 3.5835m));

        var updated = await context.Currencies.FindAsync(1);
        updated.Rate.Should().Be(3.5835m);
        updated.LastUpdatedAt.Should().NotBeNull();
    }
}
