using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class UpdateProductHandler : ICommandHandler<UpdateProductCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;  // Twoje IClock z Infrastructure

    public UpdateProductHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(UpdateProductCommand command)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == command.Id && p.RemovedAt == null)
            ?? throw new KeyNotFoundException("Product not found");

        // Walidacja unikalnego PartNumber (poza bieżącym)
        if (command.PartNumber != product.PartNumber &&
            await _dbContext.Products.AnyAsync(p => p.PartNumber == command.PartNumber && p.RemovedAt == null))
            throw new InvalidOperationException("PartNumber already exists");

        // Walidacja FK ProductGroup
        if (!await _dbContext.ProductGroups.AnyAsync(g => g.Id == command.ProductGroupId && g.RemovedAt == null))
            throw new KeyNotFoundException("ProductGroup not found or removed");

        // Aktualizacja
        product.ProductGroupId = command.ProductGroupId;
        product.PartNumber = command.PartNumber;
        product.Name = command.Name;
        product.Description = command.Description;
        product.Oembrand = command.OemBrand;
        product.ListPrice = command.ListPrice;
        product.WeightKg = command.WeightKg;
        product.LastUpdatedAt = _clock.Current();

        await _dbContext.SaveChangesAsync();
    }
}
