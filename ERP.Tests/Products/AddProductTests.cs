using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.Products;
public class AddProductTests
{
    [Fact]
    public async Task AddProduct_ShouldCreate_WhenValid()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brake Systems", CreatedAt = DateTime.UtcNow });
        await context.SaveChangesAsync();
        var clock = new Infrastructure.Time.Clock();

        var handler = new AddProductHandler(context, clock);
        var command = new AddProductCommand(1, "Brake Disc Toyota", "BRK-001", "Front discs 300mm", "Toyota", 245.99m, 8.5m);

        // Act
        await handler.HandleAsync(command);
        var id =command.Id;

        // Assert
        id.Should().BeGreaterThan(0);
        var product = await context.Products.FindAsync(id);
        product.PartNumber.Should().Be("BRK-001");
        product.ListPrice.Should().Be(245.99m);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task AddProduct_ShouldThrow_WhenPartNumberExists()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup { Id = 1, Name = "Brake Systems", CreatedAt = DateTime.UtcNow });
        context.Products.Add(new ERP.Model.Model.Product { PartNumber = "BRK-001", Name = "Brake", ProductGroupId = 1 });
        await context.SaveChangesAsync();
        var clock = new Infrastructure.Time.Clock();

        var handler = new AddProductHandler(context, clock);
        var command = new AddProductCommand(1, "Duplicate", "BRK-001", null, null, null, null);

        var act = () => handler.HandleAsync(command);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("PartNumber already exists");
    }
}
