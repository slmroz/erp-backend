using ERP.Model.Model;
using ERP.Services.Products.Queries;
using ERP.Services.Products.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.Products;
public class GetProductsTests
{
    [Fact]
    public async Task GetProducts_ShouldReturnPagedResults()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange - 3 produkty + ProductGroup
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brake Systems" });
        context.Products.AddRange(
            new Model.Model.Product { Id = 1, ProductGroupId = 1, PartNumber = "BRK001", Name = "Brake Disc Toyota", ListPrice = 245m, CreatedAt = DateTime.UtcNow },
            new Model.Model.Product { Id = 2, ProductGroupId = 1, PartNumber = "BRK002", Name = "Brake Pads VW", ListPrice = 89m, CreatedAt = DateTime.UtcNow },
            new Model.Model.Product { Id = 3, ProductGroupId = 1, PartNumber = "BRK003", Name = "Brake Caliper BMW", ListPrice = 450m, CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetProductsHandler(context);
        var query = new GetProductsQuery(Page: 1, PageSize: 2);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.Items[0].PartNumber.Should().Be("BRK001");
        result.Items[0].GroupName.Should().Be("Brake Systems");
    }

    [Fact]
    public async Task GetProducts_ShouldFilterBySearch()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brake Systems" });
        context.Products.AddRange(
            new Model.Model.Product { ProductGroupId = 1, PartNumber = "BRK001", Name = "Brake Disc", Oembrand = "Toyota", CreatedAt = DateTime.UtcNow },
            new Model.Model.Product { ProductGroupId = 1, PartNumber = "SUS001", Name = "Shock Absorber", Oembrand = "VW", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetProductsHandler(context);
        var result = await handler.HandleAsync(new GetProductsQuery(Search: "toyota"));

        result.Items.Should().HaveCount(1);
        result.Items[0].OemBrand.Should().Be("Toyota");
    }

    [Fact]
    public async Task GetProducts_ShouldFilterByGroupId()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.AddRange(
            new ProductGroup { Id = 1, Name = "Brakes" },
            new ProductGroup { Id = 2, Name = "Suspension" }
        );
        context.Products.AddRange(
            new Model.Model.Product { ProductGroupId = 1, PartNumber = "BRK001", Name = "Brake", CreatedAt = DateTime.UtcNow },
            new Model.Model.Product { ProductGroupId = 2, PartNumber = "SUS001", Name = "Suspension", CreatedAt = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        var handler = new GetProductsHandler(context);
        var result = await handler.HandleAsync(new GetProductsQuery(GroupId: 1));

        result.Items.Should().HaveCount(1);
        result.Items[0].ProductGroupId.Should().Be(1);
    }

    [Fact]
    public async Task GetProducts_ShouldExcludeRemoved()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brakes" });
        context.Products.AddRange(
            new Model.Model.Product { ProductGroupId = 1, Name = "Brake1", PartNumber = "BRK001", RemovedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
            new Model.Model.Product { ProductGroupId = 1, Name = "Brake2", PartNumber = "BRK002", CreatedAt = DateTime.UtcNow } // aktywny
        );
        await context.SaveChangesAsync();

        var handler = new GetProductsHandler(context);
        var result = await handler.HandleAsync(new GetProductsQuery());

        result.Items.Should().HaveCount(1);
        result.Items[0].PartNumber.Should().Be("BRK002");
    }
}
