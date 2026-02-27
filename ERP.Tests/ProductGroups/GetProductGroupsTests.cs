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

    [Fact]
    public async Task GetProductGroups_ShouldSortByProductCountDesc()
    {
        using var context = TestDbContextFactory.Create();

        // Grupa z wieloma produktami
        var brakes = ProductGroupTestFactory.CreateProductGroup("Brake Systems");
        context.ProductGroups.Add(brakes);
        context.Products.AddRange(

            new Product { ProductGroupId = brakes.Id, PartNumber = "BRK001", Name = "Brake1" },
            new Product { ProductGroupId = brakes.Id, PartNumber = "BRK002", Name = "Brake2" }
        );

        // Grupa z jednym produktem
        var engine = ProductGroupTestFactory.CreateProductGroup("Engine Components");
        context.ProductGroups.Add(engine);
        context.Products.Add(new Product { ProductGroupId = engine.Id, PartNumber = "ENG001", Name = "Engine" });

        await context.SaveChangesAsync();

        var handler = new GetProductGroupsHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupsQuery(SortBy: "productcount", SortOrder: "desc"));

        result.Items[0].ProductCount.Should().Be(2);  // Brake Systems
        result.Items[1].ProductCount.Should().Be(1);  // Engine Components
    }

    [Fact]
    public async Task GetProductGroups_ShouldSearchCaseInsensitive()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.AddRange(
            ProductGroupTestFactory.CreateProductGroup("BRAKE SYSTEMS"),
            ProductGroupTestFactory.CreateProductGroup("Engine Parts")
        );
        await context.SaveChangesAsync();

        var handler = new GetProductGroupsHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupsQuery(Search: "brake"));

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("BRAKE SYSTEMS");
    }

}

public static class ProductGroupTestFactory
{
    public static ProductGroup CreateProductGroup(
        string name,
        string? description = null,
        int? id = null,
        DateTime? createdAt = null,
        DateTime? lastUpdatedAt = null,
        DateTime? removedAt = null)
    {
        return new ProductGroup
        {
            Id = id ?? 0,  // 0 = IDENTITY przejmie kontrolę
            Name = name,
            Description = description,
            CreatedAt = createdAt ?? DateTime.UtcNow.AddDays(-1),
            LastUpdatedAt = lastUpdatedAt ?? DateTime.UtcNow.AddDays(-1),
            RemovedAt = removedAt
        };
    }

    // Skrócone wersje dla typowych przypadków
    public static ProductGroup CreateActive(string name) =>
        CreateProductGroup(name);

    public static ProductGroup CreateRemoved(string name) =>
        CreateProductGroup(name, removedAt: DateTime.UtcNow);
}
