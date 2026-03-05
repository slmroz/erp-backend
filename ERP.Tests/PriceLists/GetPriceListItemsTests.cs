using ERP.Model.Model;
using FluentAssertions;

namespace ERP.Tests.PriceLists;
public class GetPriceListItemsTests
{
    [Fact]
    public async Task GetPriceListItems_ShouldFilterByPriceListId()
    {
        using var context = TestDbContextFactory.Create();
        var retail = PriceListTestFactory.CreatePriceList("Retail", 1);
        var wholesale = PriceListTestFactory.CreatePriceList("Wholesale", 1);
        context.PriceLists.AddRange(retail, wholesale);
        context.Products.Add(new Product {Id = 1, PartNumber = "BRK001", Name = "Brake1" });
        context.PriceListItems.AddRange(
            PriceListTestFactory.CreatePriceListItem(retail.Id, 1, 150m),
            PriceListTestFactory.CreatePriceListItem(wholesale.Id, 1, 120m));
        await context.SaveChangesAsync();

        var handler = new GetPriceListItemsHandler(context);
        var result = await handler.HandleAsync(new GetPriceListItemsQuery(PriceListId: retail.Id));

        result.Items.Should().HaveCount(1);
        result.Items[0].PriceListName.Should().Be("Retail");
    }

    [Fact]
    public async Task GetPriceListItems_ShouldFilterByPriceRange()
    {
        using var context = TestDbContextFactory.Create();

        // ✅ Arrange - kompletny setup z FK
        var priceList = PriceListTestFactory.CreatePriceList("Retail", 1);
        context.PriceLists.Add(priceList);
        await context.SaveChangesAsync();  // Zapisz ID!

        var product1 = new Product { Id = 1, PartNumber = "BRK001", Name = "Brake Pad 100" };
        var product2 = new Product { Id = 2, PartNumber = "BRK002", Name = "Brake Pad 200" };
        var product3 = new Product { Id = 3, PartNumber = "BRK003", Name = "Brake Pad 300" };
        context.Products.AddRange(product1, product2, product3);
        await context.SaveChangesAsync();

        // 3 pozycje cenowe
        context.PriceListItems.AddRange(
            PriceListTestFactory.CreatePriceListItem(priceList.Id, 1, 100m),  // 100
            PriceListTestFactory.CreatePriceListItem(priceList.Id, 2, 200m),  // 200 ✅ w zakresie
            PriceListTestFactory.CreatePriceListItem(priceList.Id, 3, 300m)   // 300
        );
        await context.SaveChangesAsync();

        var handler = new GetPriceListItemsHandler(context);

        // Act - filtr 150-250
        var result = await handler.HandleAsync(new GetPriceListItemsQuery(
            PriceListId: priceList.Id,    // ✅ Dodane!
            MinPrice: 150m,
            MaxPrice: 250m));

        // Assert
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items[0].Price.Should().Be(200m);           // Tylko ten w zakresie
        result.Items[0].ProductName.Should().Be("Brake Pad 200");
    }

}

