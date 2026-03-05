using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

internal sealed class GetPriceListsHandler : IQueryHandler<GetPriceListsQuery, PagedResult<PriceListDto>>
{
    private readonly ErpContext _dbContext;

    public GetPriceListsHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<PriceListDto>> HandleAsync(GetPriceListsQuery query)
    {
        var baseQuery = _dbContext.PriceLists
            .Include(pl => pl.PriceListItems.Where(pi => pi.RemovedAt == null))
            .Include(pl => pl.Currency)
            .Where(pl => pl.RemovedAt == null);

        // Case-insensitive search
        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            baseQuery = baseQuery.Where(pl => pl.Name.ToLower().Contains(searchLower));
        }

        var total = await baseQuery.CountAsync();

        // ✅ Dynamiczne sortowanie
        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortOrder);

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(pl => new PriceListDto
            {
                Id = pl.Id,
                Name = pl.Name,
                Description = pl.Description,
                CreatedAt = (DateTime)pl.CreatedAt,
                CreatedBy = (int)pl.CreatedBy,
                LastUpdatedAt = pl.LastUpdatedAt,
                LastUpdatedBy = pl.LastUpdatedBy,
                RemovedAt = pl.RemovedAt,
                RemovedBy = pl.RemovedBy,
                ItemCount = pl.PriceListItems.Count,
                CurrencyId = pl.CurrencyId,
                CurrencyName = pl.Currency != null ? pl.Currency.TargetCurrency : "Unknown"
            })
            .ToListAsync();

        return new PagedResult<PriceListDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<PriceList> ApplySorting(IQueryable<PriceList> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(pl => pl.Name.ToLower()) : query.OrderBy(pl => pl.Name.ToLower()),
            "itemcount" => isDescending ? query.OrderByDescending(pl => pl.PriceListItems.Count()) : query.OrderBy(pl => pl.PriceListItems.Count()),
            "createdat" => isDescending ? query.OrderByDescending(pl => pl.CreatedAt) : query.OrderBy(pl => pl.CreatedAt),
            "createdby" => isDescending ? query.OrderByDescending(pl => pl.CreatedBy) : query.OrderBy(pl => pl.CreatedBy),
            _ => query.OrderBy(pl => pl.Name.ToLower())
        };
    }
}
