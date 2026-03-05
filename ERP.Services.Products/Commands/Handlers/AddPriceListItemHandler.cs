using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
// AddPriceListItemHandler.cs
internal sealed class AddPriceListItemHandler : ICommandHandler<AddPriceListItemCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public AddPriceListItemHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(AddPriceListItemCommand command)
    {
        // Walidacja istnienia PriceList i Product
        if (!await _dbContext.PriceLists.AnyAsync(pl => pl.Id == command.PriceListId && pl.RemovedAt == null))
            throw new KeyNotFoundException("PriceList not found");

        if (!await _dbContext.Products.AnyAsync(p => p.Id == command.ProductId && p.RemovedAt == null))
            throw new KeyNotFoundException("Product not found");

        // Walidacja unikalności (PriceListId + ProductId)
        if (await _dbContext.PriceListItems.AnyAsync(pi =>
            pi.PriceListId == command.PriceListId &&
            pi.ProductId == command.ProductId &&
            pi.RemovedAt == null))
            throw new InvalidOperationException("PriceListItem already exists");

        var item = new PriceListItem
        {
            PriceListId = command.PriceListId,
            ProductId = command.ProductId,
            Price = command.Price,
            CreatedAt = _clock.Current(),
            CreatedBy = command.CreatedBy
        };

        await _dbContext.PriceListItems.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        command.Id = item.Id; 
    }
}

