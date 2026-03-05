using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Commands.Handlers;
internal sealed class AddPriceListHandler : ICommandHandler<AddPriceListCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IClock _clock;

    public AddPriceListHandler(ErpContext dbContext, IClock clock)
    {
        _dbContext = dbContext;
        _clock = clock;
    }

    public async Task HandleAsync(AddPriceListCommand command)
    {
        // Walidacja unikalności Name
        if (await _dbContext.PriceLists.AnyAsync(pl => pl.Name == command.Name && pl.RemovedAt == null))
            throw new InvalidOperationException("PriceList name already exists");

        var priceList = new PriceList
        {
            Name = command.Name,
            Description = command.Description,
            CreatedAt = _clock.Current(),
            CreatedBy = command.CreatedBy
        };

        await _dbContext.PriceLists.AddAsync(priceList);

        if(command.FillItems)
        {
            await AddBulkPriceListItems(priceList.Id, command.CurrencyId, command.DiscountPercentage, command.CreatedBy);
        }

        await _dbContext.SaveChangesAsync();

        command.Id = priceList.Id;
    }

    private async Task AddBulkPriceListItems(int priceListId, int currencyId, decimal? discountPercentage, int createdBy)
    {
        var multiplier = await CalculatePriceMultiplier(currencyId, discountPercentage);  // 1 query!

        // ✅ Bulk insert - EF śledzi automatycznie
        var items = await _dbContext.Products
            .Where(p => p.RemovedAt == null)
            .Select(p => new PriceListItem
            {
                PriceListId = priceListId,  // ✅ Prawidłowe FK!
                ProductId = p.Id,
                Price = p.ListPrice * multiplier,
                CreatedAt = _clock.Current(),
                CreatedBy = createdBy
            })
            .ToListAsync();

        await _dbContext.PriceListItems.AddRangeAsync(items);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<decimal> CalculatePriceMultiplier(int currencyId, decimal? discountPercentage)
    {
        var currency = await _dbContext.Currencies
            .Where(c => c.Id == currencyId)
            .Select(c => c.Rate)
            .FirstAsync();

        var discountMultiplier = discountPercentage.HasValue ? (1 - discountPercentage.Value / 100) : 1m;
        return currency * discountMultiplier;
    }
}