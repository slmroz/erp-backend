using ERP.Model.Model;
using ERP.Services.Products.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests.PriceLists;
public class RemovePriceListItemTests
{
    [Fact]
    public async Task RemovePriceListItem_ShouldSoftDelete()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        // Arrange - utwórz kompletne dane
        var priceList = PriceListTestFactory.CreatePriceList("Retail", 1);
        var product = new Product { Id = 1, PartNumber = "BRK001", Name = "Brake Pad" };
        var item = PriceListTestFactory.CreatePriceListItem(priceList.Id, 1, 150m);

        context.PriceLists.Add(priceList);
        context.Products.Add(product);
        context.PriceListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new RemovePriceListItemHandler(context, clock);

        // Act
        await handler.HandleAsync(new RemovePriceListItemCommand(item.Id, 1));

        // Assert
        var removed = await context.PriceListItems.FindAsync(item.Id);
        removed.Should().NotBeNull();
        removed.RemovedAt.Should().NotBeNull();
        removed.RemovedBy.Should().Be(1);
        removed.Price.Should().Be(150m);  // Cena bez zmian

        // ✅ Nadal istnieje w DB (soft delete)
        var count = await context.PriceListItems.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task RemovePriceListItem_ShouldThrow_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new RemovePriceListItemHandler(context, clock);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(new RemovePriceListItemCommand(999, 1)));
    }

    [Fact]
    public async Task RemovePriceListItem_ShouldNotAffectAlreadyRemoved()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var item = PriceListTestFactory.CreatePriceListItem(1, 1, 100m);
        item.RemovedAt = DateTime.UtcNow.AddDays(-1);  // Już usunięty
        context.PriceListItems.Add(item);
        await context.SaveChangesAsync();

        var handler = new RemovePriceListItemHandler(context, clock);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(new RemovePriceListItemCommand(item.Id, 1)));
    }
}

