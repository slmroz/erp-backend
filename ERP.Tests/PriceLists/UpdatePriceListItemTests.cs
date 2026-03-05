using ERP.Model.Model;
using ERP.Services.Products.Commands;
using FluentAssertions;

namespace ERP.Tests.PriceLists;
public class UpdatePriceListItemTests
{
    [Fact]
    public async Task UpdatePriceListItem_ShouldModifyPrice()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var priceList = PriceListTestFactory.CreatePriceList("Retail", 1);
        var product = new Product {Id = 1, PartNumber = "BRK001", Name = "Brake1" };
        var item = PriceListTestFactory.CreatePriceListItem(priceList.Id, 1, 100m);
        context.PriceLists.Add(priceList);
        context.Products.Add(product);
        context.PriceListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new UpdatePriceListItemHandler(context, clock);
        await handler.HandleAsync(new UpdatePriceListItemCommand(item.Id, priceList.Id, 1, 250.00m, 1));

        var updated = await context.PriceListItems.FindAsync(item.Id);
        updated.Price.Should().Be(250.00m);
        updated.LastUpdatedAt.Should().NotBeNull();
    }
}

