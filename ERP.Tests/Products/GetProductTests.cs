using ERP.Model.Model;
using ERP.Services.Products.Queries;
using ERP.Services.Products.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Products;
public class GetProductTests
{
    [Fact]
    public async Task GetProduct_ShouldReturnDto_WhenExists()
    {
        using var context = TestDbContextFactory.Create();

        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brake Systems" });
        var product = new Model.Model.Product
        {
            Id = 1,
            ProductGroupId = 1,
            PartNumber = "BRK001",
            Name = "Brake Disc Toyota",
            ListPrice = 245.99m,
            WeightKg = 8.5m,
            CreatedAt = DateTime.UtcNow
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new GetProductHandler(context);
        var result = await handler.HandleAsync(new GetProductQuery(1));

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.PartNumber.Should().Be("BRK001");
        result.GroupName.Should().Be("Brake Systems");
        result.ListPrice.Should().Be(245.99m);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnNull_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetProductHandler(context);
        var result = await handler.HandleAsync(new GetProductQuery(999));

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProduct_ShouldExcludeRemoved()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brakes" });
        context.Products.Add(new Model.Model.Product
        {
            Id = 1,
            ProductGroupId = 1,
            PartNumber = "BRK001",
            RemovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Name = "Brake1"
        });
        await context.SaveChangesAsync();

        var handler = new GetProductHandler(context);
        var result = await handler.HandleAsync(new GetProductQuery(1));

        result.Should().BeNull();
    }
}
