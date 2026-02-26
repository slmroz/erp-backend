using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Currencies;
public class AddCurrencyTests
{
    [Fact]
    public async Task AddCurrency_ShouldCreate_WhenValid()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new AddCurrencyHandler(context);
        var command = new AddCurrencyCommand("USD", "PLN", 3.5835m);

        await handler.HandleAsync(command);
        var id = command.Id;

        id.Should().BeGreaterThan(0);
        var currency = await context.Currencies.FindAsync(id);
        currency.Should().NotBeNull();
        currency.BaseCurrency.Should().Be("USD");
        currency.TargetCurrency.Should().Be("PLN");
        currency.Rate.Should().Be(3.5835m);
    }

    [Fact]
    public async Task AddCurrency_ShouldThrow_WhenPairExists()
    {
        using var context = TestDbContextFactory.Create();
        context.Currencies.Add(new Currency { BaseCurrency = "USD", TargetCurrency = "PLN" });
        await context.SaveChangesAsync();

        var handler = new AddCurrencyHandler(context);
        var act = () => handler.HandleAsync(new AddCurrencyCommand("USD", "PLN", 3.5835m));

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*USD|PLN*already exists*");
    }
}
