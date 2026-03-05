using ERP.Model.Model;
using FluentAssertions;

namespace ERP.Tests.PriceLists;
public class GetPriceListsTests
{
    [Fact]
    public async Task GetPriceLists_ShouldReturnPagedWithItemCount()
    {
        using var context = TestDbContextFactory.Create();

        // ✅ Currency setup
        var currency = new Currency { Id = 1, BaseCurrency = "USD", TargetCurrency = "PLN" };
        context.Currencies.Add(currency);
        await context.SaveChangesAsync();

        // ✅ 1. Najpierw zapisz PriceList (ID = 1)
        var retail = PriceListTestFactory.CreatePriceList("Retail", currency.Id);
        context.PriceLists.Add(retail);
        await context.SaveChangesAsync();  // ← BRAKOWAŁO!

        // ✅ 2. Teraz Product (ID = 1)
        var product = new Product { Id = 1, PartNumber = "BRK001", Name = "Brake Pad" };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // ✅ 3. TERAZ PriceListItem z prawdziwymi ID
        context.PriceListItems.Add(PriceListTestFactory.CreatePriceListItem(retail.Id, product.Id, 100m));
        await context.SaveChangesAsync();

        var handler = new GetPriceListsHandler(context);
        var result = await handler.HandleAsync(new GetPriceListsQuery(PageSize: 10));

        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items[0].ItemCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPriceLists_ShouldSortByItemCountDesc()
    {
        using var context = TestDbContextFactory.Create();

        // ✅ Currency setup
        var currency = new Currency { Id = 1, BaseCurrency = "USD", TargetCurrency = "PLN" };
        context.Currencies.Add(currency);
        await context.SaveChangesAsync();


        var retail = PriceListTestFactory.CreatePriceList("Retail", currency.Id);
        var wholesale = PriceListTestFactory.CreatePriceList("Wholesale", currency.Id);
        context.PriceLists.AddRange(retail, wholesale);
        await context.SaveChangesAsync();

        context.PriceListItems.AddRange(
            PriceListTestFactory.CreatePriceListItem(retail.Id, 1, 100m),
            PriceListTestFactory.CreatePriceListItem(retail.Id, 2, 200m));  // 2 items
        await context.SaveChangesAsync();

        var handler = new GetPriceListsHandler(context);
        var result = await handler.HandleAsync(new GetPriceListsQuery(SortBy: "itemcount", SortOrder: "desc"));

        result.Items[0].Name.Should().Be("Retail");  // Więcej items
    }
}
