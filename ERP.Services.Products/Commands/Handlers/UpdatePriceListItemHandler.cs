using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.Commands;
using Microsoft.EntityFrameworkCore;

internal sealed class UpdatePriceListItemHandler : ICommandHandler<UpdatePriceListItemCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public UpdatePriceListItemHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(UpdatePriceListItemCommand command)
    {
        var item = await _dbContext.PriceListItems
            .FirstOrDefaultAsync(pi => pi.Id == command.Id && pi.RemovedAt == null)
            ?? throw new KeyNotFoundException("PriceListItem not found");

        // Walidacja istnienia PriceList i Product
        if (!await _dbContext.PriceLists.AnyAsync(pl => pl.Id == command.PriceListId && pl.RemovedAt == null))
            throw new KeyNotFoundException("PriceList not found");

        if (!await _dbContext.Products.AnyAsync(p => p.Id == command.ProductId && p.RemovedAt == null))
            throw new KeyNotFoundException("Product not found");

        // Walidacja unikalności (PriceListId + ProductId - poza bieżącym)
        if (command.Id != item.Id &&
            await _dbContext.PriceListItems.AnyAsync(pi =>
                pi.PriceListId == command.PriceListId &&
                pi.ProductId == command.ProductId &&
                pi.RemovedAt == null))
            throw new InvalidOperationException("PriceListItem already exists for this PriceList/Product");

        item.PriceListId = command.PriceListId;
        item.ProductId = command.ProductId;
        item.Price = command.Price;
        item.LastUpdatedAt = _clock.Current();
        item.LastUpdatedBy = command.LastUpdatedBy;

        await _dbContext.SaveChangesAsync();
    }
}
