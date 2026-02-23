using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.ProductGroups;
public class RemoveProductGroupTests
{
    [Fact]
    public async Task RemoveProductGroup_ShouldSoftDelete()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var group = new ProductGroup { Id = 1, Name = "Brake Systems" };
        context.ProductGroups.Add(group);
        await context.SaveChangesAsync();

        var handler = new RemoveProductGroupHandler(context, clock);
        await handler.HandleAsync(new RemoveProductGroupCommand(1));

        var removed = await context.ProductGroups.FindAsync(1);
        removed.RemovedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveProductGroup_ShouldThrow_WhenProductsExist()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brakes" });
        context.Products.Add(new Product { ProductGroupId = 1, PartNumber = "BRK001" });
        await context.SaveChangesAsync();

        var handler = new RemoveProductGroupHandler(context, clock);
        var act = () => handler.HandleAsync(new RemoveProductGroupCommand(1));

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Cannot remove group with active products");
    }

    [Fact]
    public async Task RemoveProductGroup_ShouldThrow_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new RemoveProductGroupHandler(context, clock);
        var act = () => handler.HandleAsync(new RemoveProductGroupCommand(999));

        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage("ProductGroup not found");
    }
}
