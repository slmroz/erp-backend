using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests.PriceLists;
public class AddPriceListTests
{
    [Fact]
    public async Task AddPriceList_ShouldCreateNew()
    {
        var clock = new Infrastructure.Time.Clock();
        using var context = TestDbContextFactory.Create();

        // ✅ Currency setup
        var currency = new Currency { Id = 1, BaseCurrency = "USD", TargetCurrency = "PLN" };
        context.Currencies.Add(currency);
        var handler = new AddPriceListHandler(context, clock);

        await handler.HandleAsync(new AddPriceListCommand("Retail", "Test", 1, 1, null, false));

        var saved = await context.PriceLists.FirstAsync();
        saved.Name.Should().Be("Retail");
        saved.CreatedBy.Should().Be(1);
        saved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task AddPriceList_ShouldThrow_WhenNameExists()
    {
        var clock = new Infrastructure.Time.Clock();
        using var context = TestDbContextFactory.Create();
        // ✅ Currency setup
        var currency = new Currency { Id = 1, BaseCurrency = "USD", TargetCurrency = "PLN" };
        context.Currencies.Add(currency);
        await context.PriceLists.AddAsync(PriceListTestFactory.CreatePriceList("Retail", 1));
        await context.SaveChangesAsync();

        var handler = new AddPriceListHandler(context, clock);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(new AddPriceListCommand("Retail", null, 1, 1, null, false)));
    }

    [Fact]
    public async Task AddPriceList_WithFillItems_ShouldCreateBulkItems()
    {
        var clock = new ERP.Infrastructure.Time.Clock();
        using var context = TestDbContextFactory.Create();

        // ✅ Setup - TYLKO te produkty mają być użyte
        var pln = new Currency { Id = 1, BaseCurrency = "USD", TargetCurrency = "PLN", Rate = 1m };
        var products = new[]
        {
        new Product
        {
            Id = 1,
            Name = "Brake Pad",           // ✅ Wymagane!
            PartNumber = "BRK001",        // ✅ Wymagane!
            ListPrice = 100m
        },
        new Product
        {
            Id = 2,
            Name = "Disc Brake",          // ✅ Wymagane!
            PartNumber = "BRK002",        // ✅ Wymagane!
            ListPrice = 200m
        }
    };

        context.Currencies.Add(pln);
        context.Products.AddRange(products);
        await context.SaveChangesAsync();

        var handler = new AddPriceListHandler(context, clock);

        // Act
        await handler.HandleAsync(new AddPriceListCommand(
            Name: "Retail",
            Description: "Test",
            CreatedBy: 1,
            CurrencyId: 1,
            DiscountPercentage: 5m,
            FillItems: true));

        // Assert - SPRAWDŹ po ID produktów
        var items = await context.PriceListItems
            .Where(pi => pi.ProductId == 1 || pi.ProductId == 2)
            .OrderBy(pi => pi.ProductId)
            .ToListAsync();

        items.Should().HaveCount(2);
        items[0].Price.Should().Be(95m);        // 100 * 1 * 0.95
        items[1].Price.Should().Be(190m);       // 200 * 1 * 0.95
    }


}
