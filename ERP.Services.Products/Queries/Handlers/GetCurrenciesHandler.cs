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
        var filterQuery = _dbContext.Currencies.Where(c => c.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.BaseCurrency))
            filterQuery = filterQuery.Where(c => c.BaseCurrency == query.BaseCurrency.ToUpper());

        if (!string.IsNullOrEmpty(query.TargetCurrency))
            filterQuery = filterQuery.Where(c => c.TargetCurrency == query.TargetCurrency.ToUpper());

        var total = await filterQuery.CountAsync();

        var items = await filterQuery
            .OrderBy(c => c.BaseCurrency).ThenBy(c => c.TargetCurrency)
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
}
