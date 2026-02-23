using ERP.Model.Model;
using ERP.Services.Products.Commands;
using ERP.Services.Products.Commands.Handlers;
using FluentAssertions;

namespace ERP.Tests.ProductGroups;
public class AddProductGroupTests
{
    [Fact]
    public async Task AddProductGroup_ShouldCreate_WhenValid()
    {
        using var context = TestDbContextFactory.Create();
        var handler = new AddProductGroupHandler(context);
        var command = new AddProductGroupCommand("Brake Systems", "Układy hamulcowe");

        await handler.HandleAsync(command);
        var id = command.Id;

        id.Should().BeGreaterThan(0);
        var group = await context.ProductGroups.FindAsync(id);
        group.Should().NotBeNull();
        group.Name.Should().Be("Brake Systems");
        group.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task AddProductGroup_ShouldThrow_WhenNameExists()
    {
        using var context = TestDbContextFactory.Create();
        context.ProductGroups.Add(new ProductGroup { Name = "Brake Systems" });
        await context.SaveChangesAsync();

        var handler = new AddProductGroupHandler(context);
        var act = () => handler.HandleAsync(new AddProductGroupCommand("Brake Systems", "Test"));

        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ProductGroup name already exists");
    }
}
