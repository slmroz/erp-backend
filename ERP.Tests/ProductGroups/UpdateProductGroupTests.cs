using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.ProductGroups;
public class UpdateProductGroupTests
{
    [Fact]
    public async Task UpdateProductGroup_ShouldModifyFields()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        var group = new ProductGroup
        {
            Id = 1,
            Name = "Old Brakes",
            Description = "Old desc",
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        context.ProductGroups.Add(group);
        await context.SaveChangesAsync();

        var handler = new UpdateProductGroupHandler(context, clock);
        var command = new UpdateProductGroupCommand(1, "Brake Systems", "Updated hamulce");

        await handler.HandleAsync(command);

        var updated = await context.ProductGroups.FindAsync(1);
        updated.Name.Should().Be("Brake Systems");
        updated.Description.Should().Be("Updated hamulce");
        updated.LastUpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateProductGroup_ShouldThrow_WhenNameExists()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();

        context.ProductGroups.AddRange(
            new ProductGroup { Name = "Old Brakes" },
            new ProductGroup { Name = "Brake Systems" }
        );
        await context.SaveChangesAsync();

        var handler = new UpdateProductGroupHandler(context, clock);
        var act = () => handler.HandleAsync(new UpdateProductGroupCommand(1, "Brake Systems", "Test"));

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ProductGroup name already exists");
    }

    [Fact]
    public async Task UpdateProductGroup_ShouldThrow_WhenNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var clock = new ERP.Infrastructure.Time.Clock();
        var handler = new UpdateProductGroupHandler(context, clock);
        var act = () => handler.HandleAsync(new UpdateProductGroupCommand(999, "Test", null));

        await act.Should().ThrowAsync<KeyNotFoundException>()
                 .WithMessage("ProductGroup not found");
    }
}
