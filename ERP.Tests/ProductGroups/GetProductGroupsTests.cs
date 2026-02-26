using ERP.Model.Model;
using ERP.Services.Products.Queries;
using ERP.Services.Products.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.ProductGroups;
public class GetProductGroupsTests
{
    [Fact]
    public async Task GetProductGroups_ShouldReturnPagedResults()
    {
        using var context = TestDbContextFactory.Create();

        context.ProductGroups.AddRange(
            new ProductGroup { Id = 1, Name = "Brake Systems", CreatedAt = DateTime.UtcNow },
            new ProductGroup { Id = 2, Name = "Engine Components", CreatedAt = DateTime.UtcNow },
            new ProductGroup { Id = 3, Name = "Suspension", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetProductGroupsHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupsQuery(1, 2));

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.Items[0].ProductCount.Should().Be(0);  // Bez produktów
    }

    [Fact]
    public async Task GetProductGroups_ShouldFilterBySearch()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.AddRange(
            new ProductGroup { Id = 1, Name = "Brake Systems", CreatedAt = DateTime.UtcNow },
            new ProductGroup { Id = 2, Name = "Engine Components", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetProductGroupsHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupsQuery(Search: "brake"));

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Brake Systems");
    }

    [Fact]
    public async Task GetProductGroups_ShouldIncludeProductCount()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brakes", CreatedAt = DateTime.UtcNow });
        context.Products.Add(new Product { ProductGroupId = 1, Name = "Brake", PartNumber = "BRK001", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var handler = new GetProductGroupsHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupsQuery());

        result.Items.Should().HaveCount(1);
        result.Items[0].ProductCount.Should().Be(1);
    }
}
