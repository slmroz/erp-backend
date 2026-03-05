using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class UpdatePriceListHandler : ICommandHandler<UpdatePriceListCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public UpdatePriceListHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(UpdatePriceListCommand command)
    {
        var priceList = await _dbContext.PriceLists
            .FirstOrDefaultAsync(pl => pl.Id == command.Id && pl.RemovedAt == null)
            ?? throw new KeyNotFoundException("PriceList not found");

        if (command.Name != priceList.Name &&
            await _dbContext.PriceLists.AnyAsync(pl => pl.Name == command.Name && pl.RemovedAt == null))
            throw new InvalidOperationException("PriceList name already exists");

        priceList.Name = command.Name;
        priceList.Description = command.Description;
        priceList.LastUpdatedAt = _clock.Current();
        priceList.LastUpdatedBy = command.LastUpdatedBy;

        await _dbContext.SaveChangesAsync();
    }
}