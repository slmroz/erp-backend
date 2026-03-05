using ERP.Model.Model;
using ERP.Services.Products.Queries;
using ERP.Services.Products.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.PriceLists;
public class GetPriceListTests
{
    [Fact]
    public async Task GetPriceList_ShouldReturnDtoWithItemCount()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange - kompletny setup
        var priceList = PriceListTestFactory.CreatePriceList("Retail", 1);
        context.PriceLists.Add(priceList);
        await context.SaveChangesAsync();

        context.Products.Add(new Product { Id = 1, PartNumber = "BRK001", Name = "Brake Pad" });
        await context.SaveChangesAsync();

        context.PriceListItems.Add(PriceListTestFactory.CreatePriceListItem(priceList.Id, 1, 150m));
        await context.SaveChangesAsync();

        var handler = new GetPriceListHandler(context);

        // Act
        var result = await handler.HandleAsync(new GetPriceListQuery(priceList.Id));

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Retail");
        result.ItemCount.Should().Be(1);           // ✅ Licznik z Include
        result.CreatedBy.Should().Be(1);
    }

    [Fact]
    public async Task GetPriceList_ShouldReturnNull_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetPriceListHandler(context);

        var result = await handler.HandleAsync(new GetPriceListQuery(999));

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPriceList_ShouldReturnNull_WhenRemoved()
    {
        using var context = TestDbContextFactory.Create();

        var priceList = PriceListTestFactory.CreatePriceList("Removed", 1);
        priceList.RemovedAt = DateTime.UtcNow.AddDays(-1);  // Soft delete
        context.PriceLists.Add(priceList);
        await context.SaveChangesAsync();

        var handler = new GetPriceListHandler(context);

        var result = await handler.HandleAsync(new GetPriceListQuery(priceList.Id));

        result.Should().BeNull();  // ✅ Where(pl => pl.RemovedAt == null)
    }
}
