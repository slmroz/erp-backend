using ERP.Model.Model;
using ERP.Services.Products.Queries;
using ERP.Services.Products.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Currencies;
public class GetCurrenciesTests
{
    [Fact]
    public async Task GetCurrencies_ShouldFilterByBaseCurrency()
    {
        using var context = TestDbContextFactory.Create();
        context.Currencies.AddRange(
            new Currency
            {
                BaseCurrency = "USD",
                TargetCurrency = "PLN",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                RemovedAt = null  // Explicit null OK
            },
            new Currency
            {
                BaseCurrency = "EUR",
                TargetCurrency = "PLN",
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                RemovedAt = null
            }
        );
        await context.SaveChangesAsync();

        var handler = new GetCurrenciesHandler(context);
        var result = await handler.HandleAsync(new GetCurrenciesQuery(BaseCurrency: "USD"));

        result.Items.Should().HaveCount(1);
        result.Items[0].BaseCurrency.Should().Be("USD");
    }
}