using ERP.Model.Model;
using ERP.Services.Products.Queries;
using ERP.Services.Products.Queries.Handlers;
using FluentAssertions;

namespace ERP.Tests.ProductGroups;
public class GetProductGroupTests
{
    [Fact]
    public async Task GetProductGroup_ShouldReturnDto()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup
        {
            Id = 1,
            Name = "Brake Systems",
            Description = "Hamulce",
            CreatedAt = DateTime.UtcNow
        });
        context.Products.Add(new Product { ProductGroupId = 1 });
        await context.SaveChangesAsync();

        var handler = new GetProductGroupHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupQuery(1));

        result.Should().NotBeNull();
        result.Name.Should().Be("Brake Systems");
        result.ProductCount.Should().Be(1);
    }

    [Fact]
    public async Task GetProductGroup_ShouldReturnNull_NotFound()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new GetProductGroupHandler(context);
        var result = await handler.HandleAsync(new GetProductGroupQuery(999));

        result.Should().BeNull();
    }
}
