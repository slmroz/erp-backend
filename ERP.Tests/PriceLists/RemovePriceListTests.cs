using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.PriceLists;
public class RemovePriceListTests
{
    [Fact]
    public async Task RemovePriceList_ShouldSoftDelete_WhenNoItems()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var priceList = PriceListTestFactory.CreatePriceList("Test", 1);
        context.PriceLists.Add(priceList);
        await context.SaveChangesAsync();

        var handler = new RemovePriceListHandler(context, clock);
        await handler.HandleAsync(new RemovePriceListCommand(priceList.Id, 1));

        var removed = await context.PriceLists.FindAsync(priceList.Id);
        removed.RemovedAt.Should().NotBeNull();
        removed.RemovedBy.Should().Be(1);
    }

    [Fact]
    public async Task RemovePriceList_ShouldThrow_WhenHasItems()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var priceList = PriceListTestFactory.CreatePriceList("Test", 1);
        context.PriceLists.Add(priceList);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Id = 1, PartNumber = "BRK001", Name = "Brake1" });
        context.PriceListItems.Add(PriceListTestFactory.CreatePriceListItem(priceList.Id, 1, 100m));
        await context.SaveChangesAsync();

        var handler = new RemovePriceListHandler(context, clock);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(new RemovePriceListCommand(priceList.Id, 1)));
    }
}
