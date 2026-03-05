using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.PriceLists;
public class UpdatePriceListTests
{
    [Fact]
    public async Task UpdatePriceList_ShouldModifyFields()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var priceList = PriceListTestFactory.CreatePriceList("Old Name", 1);
        context.PriceLists.Add(priceList);
        await context.SaveChangesAsync();

        var handler = new UpdatePriceListHandler(context, clock);
        await handler.HandleAsync(new UpdatePriceListCommand(priceList.Id, "New Name", "New Desc", 1));

        var updated = await context.PriceLists.FindAsync(priceList.Id);
        updated.Name.Should().Be("New Name");
        updated.LastUpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatePriceList_ShouldThrow_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdatePriceListHandler(context, clock);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => handler.HandleAsync(new UpdatePriceListCommand(999, "New", null, 1)));
    }
}
