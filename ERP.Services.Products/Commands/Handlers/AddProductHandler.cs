using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;

internal sealed class AddProductHandler : ICommandHandler<AddProductCommand>
{
    private readonly ErpContext _dbContext;
    public AddProductHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task HandleAsync(AddProductCommand command)
    {
        // Walidacja FK ProductGroup + unikalny PartNumber
        if (!await _dbContext.ProductGroups.AnyAsync(g => g.Id == command.ProductGroupId && g.RemovedAt == null))
            throw new KeyNotFoundException("ProductGroup not found or removed");

        if (await _dbContext.Products.AnyAsync(p => p.PartNumber == command.PartNumber && p.RemovedAt == null))
            throw new InvalidOperationException("PartNumber already exists");

        var product = new Product
        {
            ProductGroupId = command.ProductGroupId,
            PartNumber = command.PartNumber,
            Name = command.Name,
            Description = command.Description,
            Oembrand = command.OemBrand,
            ListPrice = command.ListPrice,
            WeightKg = command.WeightKg,
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        command.Id = product.Id;
    }
}
