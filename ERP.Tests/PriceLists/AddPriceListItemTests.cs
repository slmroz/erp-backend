using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests.PriceLists;
public class AddPriceListItemTests
{
    [Fact]
    public async Task AddPriceListItem_ShouldCreateNew()
    {
        var clock = new Infrastructure.Time.Clock();
        using var context = TestDbContextFactory.Create();
        var priceList = PriceListTestFactory.CreatePriceList("Retail", 1);
        var product = new Product { Id = 1, PartNumber = "BRK001", Name = "Brake1" };
        context.PriceLists.Add(priceList);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new AddPriceListItemHandler(context, clock);
        await handler.HandleAsync(new AddPriceListItemCommand(priceList.Id, product.Id, 150.00m, 1));

        var saved = await context.PriceListItems.FirstAsync();
        saved.Price.Should().Be(150.00m);
        saved.PriceListId.Should().Be(priceList.Id);
    }

    [Fact]
    public async Task AddPriceListItem_ShouldThrow_WhenPriceListNotFound()
    {
        var clock = new Infrastructure.Time.Clock();
        using var context = TestDbContextFactory.Create();
        context.Products.Add(new Product {Id = 1, PartNumber = "BRK001", Name = "Brake1" });
        await context.SaveChangesAsync();

        var handler = new AddPriceListItemHandler(context, clock);
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(new AddPriceListItemCommand(999, 1, 100m, 1)));
    }

    [Fact]
    public async Task AddPriceListItem_ShouldThrow_WhenDuplicate()
    {
        var clock = new Infrastructure.Time.Clock();
        using var context = TestDbContextFactory.Create();
        var priceList = PriceListTestFactory.CreatePriceList("Retail", 1);
        var product = new Product { Id = 1, PartNumber = "BRK001", Name = "Brake1" };
        context.PriceLists.Add(priceList);
        context.Products.Add(product);
        context.PriceListItems.Add(PriceListTestFactory.CreatePriceListItem(priceList.Id, 1, 100m));
        await context.SaveChangesAsync();

        var handler = new AddPriceListItemHandler(context, clock);
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(new AddPriceListItemCommand(priceList.Id, 1, 200m, 1)));
    }
}
