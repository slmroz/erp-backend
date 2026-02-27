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

    [Fact]
    public async Task GetCurrencies_ShouldFilterCaseInsensitive()
    {
        using var context = TestDbContextFactory.Create();
        context.Currencies.AddRange(
            new Currency { BaseCurrency = "USD", TargetCurrency = "PLN", Rate = 3.58m, CreatedAt = DateTime.UtcNow },
            new Currency { BaseCurrency = "usd", TargetCurrency = "EUR", Rate = 0.85m, CreatedAt = DateTime.UtcNow },
            new Currency { BaseCurrency = "EUR", TargetCurrency = "PLN", Rate = 3.05m, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetCurrenciesHandler(context);
        var result = await handler.HandleAsync(new GetCurrenciesQuery(BaseCurrency: "UsD"));

        result.Items.Should().HaveCount(2);  // USD/pln + usd/eur
    }

    [Fact]
    public async Task GetCurrencies_ShouldSortByRateDesc()
    {
        using var context = TestDbContextFactory.Create();
        context.Currencies.AddRange(
            new Currency { BaseCurrency = "USD", TargetCurrency = "PLN", Rate = 3.58m, CreatedAt = DateTime.UtcNow },
            new Currency { BaseCurrency = "USD", TargetCurrency = "JPY", Rate = 155.84m, CreatedAt = DateTime.UtcNow },
            new Currency { BaseCurrency = "EUR", TargetCurrency = "USD", Rate = 1.18m, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetCurrenciesHandler(context);
        var result = await handler.HandleAsync(new GetCurrenciesQuery(SortBy: "rate", SortOrder: "desc"));

        result.Items[0].Rate.Should().Be(155.84m);  // JPY najwyższy
        result.Items[1].Rate.Should().Be(3.58m);
        result.Items[2].Rate.Should().Be(1.18m);
    }

}

public static class CurrencyTestFactory
{
    public static Currency CreateCurrency(string baseCurrency, string targetCurrency, decimal rate)
    {
        return new Currency
        {
            BaseCurrency = baseCurrency.ToUpper(),
            TargetCurrency = targetCurrency.ToUpper(),
            Rate = rate,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            LastUpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
    }
}