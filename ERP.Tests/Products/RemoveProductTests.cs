using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ERP.Tests.Products;
public class RemoveProductTests
{
    [Fact]
    public async Task RemoveProduct_ShouldSoftDelete()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var product = new Model.Model.Product { Id = 1, ProductGroupId = 1, PartNumber = "BRK-001" };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new RemoveProductHandler(context, clock);
        await handler.HandleAsync(new RemoveProductCommand(1));

        var removed = await context.Products.FindAsync(1);
        removed.RemovedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveProduct_ShouldThrow_WhenUsedInOpportunities()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        context.Products.Add(new Model.Model.Product { Id = 1, ProductGroupId = 1 });
        //context.Opportunities.Add(new Opportunity { Id = 1, ProductId = 1 }); // FK
        await context.SaveChangesAsync();

        var handler = new RemoveProductHandler(context, clock);

        var act = () => handler.HandleAsync(new RemoveProductCommand(1));
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Cannot remove product used in active opportunities");
    }
}

