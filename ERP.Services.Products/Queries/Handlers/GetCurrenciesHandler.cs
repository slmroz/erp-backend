using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Queries.Handlers;
internal sealed class GetCurrenciesHandler : IQueryHandler<GetCurrenciesQuery, PagedResult<CurrencyDto>>
{
    private readonly ErpContext _dbContext;

    public GetCurrenciesHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<CurrencyDto>> HandleAsync(GetCurrenciesQuery query)
    {
        var baseQuery = _dbContext.Currencies.Where(c => c.RemovedAt == null);

        // ✅ Case-insensitive filtry
        if (!string.IsNullOrEmpty(query.BaseCurrency))
            baseQuery = baseQuery.Where(c => c.BaseCurrency.ToUpper() == query.BaseCurrency.ToUpper());

        if (!string.IsNullOrEmpty(query.TargetCurrency))
            baseQuery = baseQuery.Where(c => c.TargetCurrency.ToUpper() == query.TargetCurrency.ToUpper());

        var total = await baseQuery.CountAsync();

        // ✅ Dynamiczne sortowanie
        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortOrder);

        var items = await baseQuery
            //.OrderBy(c => c.BaseCurrency).ThenBy(c => c.TargetCurrency)  // Fallback
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CurrencyDto
            {
                Id = c.Id,
                BaseCurrency = c.BaseCurrency,
                TargetCurrency = c.TargetCurrency,
                Rate = c.Rate,
                CreatedAt = (DateTime)c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt,
                RemovedAt = c.RemovedAt
            })
            .ToListAsync();

        return new PagedResult<CurrencyDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<Currency> ApplySorting(IQueryable<Currency> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "basecurrency" => isDescending ? query.OrderByDescending(c => c.BaseCurrency.ToUpper()) : query.OrderBy(c => c.BaseCurrency.ToUpper()),
            "targetcurrency" => isDescending ? query.OrderByDescending(c => c.TargetCurrency.ToUpper()) : query.OrderBy(c => c.TargetCurrency.ToUpper()),
            "rate" => isDescending ? query.OrderByDescending(c => c.Rate) : query.OrderBy(c => c.Rate),
            "createdat" => isDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
            "lastupdatedat" => isDescending ? query.OrderByDescending(c => c.LastUpdatedAt) : query.OrderBy(c => c.LastUpdatedAt),
            _ => query.OrderBy(c => c.BaseCurrency.ToUpper()).ThenBy(c => c.TargetCurrency.ToUpper())
        };
    }
}
