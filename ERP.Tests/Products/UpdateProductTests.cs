using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Products;
public class UpdateProductTests
{
    [Fact]
    public async Task UpdateProduct_ShouldModifyFields()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brake Systems", CreatedAt = DateTime.UtcNow });
        var product = new ERP.Model.Model.Product
        {
            Id = 1,
            ProductGroupId = 1,
            PartNumber = "OLD-001",
            Name = "Old Name",
            ListPrice = 100m,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var handler = new UpdateProductHandler(context, clock);
        var command = new UpdateProductCommand(1, 1, "BRK-001", "New Brake Disc", "Updated desc", "Toyota", 245.99m, 8.5m);

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updated = await context.Products.FindAsync(1);
        updated.Name.Should().Be("New Brake Disc");
        updated.ListPrice.Should().Be(245.99m);
        updated.LastUpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateProduct_ShouldThrow_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdateProductHandler(context, clock);
        var command = new UpdateProductCommand(999, 1, "BRK-001", "Test", null, null, null, null);

        var act = () => handler.HandleAsync(command);
        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage("Product not found");
    }
}

