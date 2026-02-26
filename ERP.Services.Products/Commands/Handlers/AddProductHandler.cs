using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;

internal sealed class AddProductHandler : ICommandHandler<AddProductCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _dbContext;
    public AddProductHandler(ErpContext dbContext, IClock clock)
    {
        _clock = clock;
        _dbContext = dbContext;
    }

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
            CreatedAt = _clock.Current(),
            LastUpdatedAt = _clock.Current()
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        command.Id = product.Id;
    }
}
