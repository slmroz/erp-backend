using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class RemovePriceListHandler : ICommandHandler<RemovePriceListCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public RemovePriceListHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(RemovePriceListCommand command)
    {
        var priceList = await _dbContext.PriceLists
            .FirstOrDefaultAsync(pl => pl.Id == command.Id && pl.RemovedAt == null)
            ?? throw new KeyNotFoundException("PriceList not found");

        // Walidacja: brak aktywnych pozycji cenowych
        if (await _dbContext.PriceListItems.AnyAsync(pi => pi.PriceListId == command.Id && pi.RemovedAt == null))
            throw new InvalidOperationException("Cannot remove PriceList with active items");

        priceList.RemovedAt = _clock.Current();
        priceList.RemovedBy = command.RemovedBy;

        await _dbContext.SaveChangesAsync();
    }
}