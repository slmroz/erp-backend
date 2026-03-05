using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.Commands;
using Microsoft.EntityFrameworkCore;

internal sealed class RemovePriceListItemHandler : ICommandHandler<RemovePriceListItemCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public RemovePriceListItemHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(RemovePriceListItemCommand command)
    {
        var item = await _dbContext.PriceListItems
            .FirstOrDefaultAsync(pi => pi.Id == command.Id && pi.RemovedAt == null)
            ?? throw new KeyNotFoundException("PriceListItem not found");

        item.RemovedAt = _clock.Current();
        item.RemovedBy = command.RemovedBy;

        await _dbContext.SaveChangesAsync();
    }
}
