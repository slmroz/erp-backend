using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

internal sealed class GetPriceListItemsHandler : IQueryHandler<GetPriceListItemsQuery, PagedResult<PriceListItemDto>>
{
    private readonly ErpContext _dbContext;

    public GetPriceListItemsHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<PriceListItemDto>> HandleAsync(GetPriceListItemsQuery query)
    {
        var baseQuery = _dbContext.PriceListItems
            .Include(pi => pi.PriceList)
            .Include(pi => pi.Product)
            .Where(pi => pi.RemovedAt == null);

        // ✅ Filtry
        if (query.PriceListId.HasValue)
            baseQuery = baseQuery.Where(pi => pi.PriceListId == query.PriceListId);

        if (query.MinPrice.HasValue)
            baseQuery = baseQuery.Where(pi => pi.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            baseQuery = baseQuery.Where(pi => pi.Price <= query.MaxPrice.Value);

        var total = await baseQuery.CountAsync();

        // ✅ Dynamiczne sortowanie
        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortOrder);

        var tmp = _dbContext.PriceListItems.Include(p => p.PriceList).Include(p => p.Product).Count();

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(pi => new PriceListItemDto
            {
                Id = pi.Id,
                PriceListId = pi.PriceListId,
                PriceListName = pi.PriceList == null ? null : pi.PriceList.Name,
                ProductId = pi.ProductId,
                ProductName = pi.Product == null ? null : pi.Product.Name,
                Price = (decimal)pi.Price,
                CreatedAt = (DateTime)pi.CreatedAt,
                CreatedBy = pi.CreatedBy,
                LastUpdatedAt = pi.LastUpdatedAt,
                LastUpdatedBy = pi.LastUpdatedBy,
                RemovedAt = pi.RemovedAt,
                RemovedBy = pi.RemovedBy
            })
            .ToListAsync();
        
        return new PagedResult<PriceListItemDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<PriceListItem> ApplySorting(IQueryable<PriceListItem> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "pricelistname" => isDescending ? query.OrderByDescending(pi => pi.PriceList.Name.ToLower()) : query.OrderBy(pi => pi.PriceList.Name.ToLower()),
            "productname" => isDescending ? query.OrderByDescending(pi => pi.Product.Name.ToLower()) : query.OrderBy(pi => pi.Product.Name.ToLower()),
            "price" => isDescending ? query.OrderByDescending(pi => pi.Price) : query.OrderBy(pi => pi.Price),
            "createdat" => isDescending ? query.OrderByDescending(pi => pi.CreatedAt) : query.OrderBy(pi => pi.CreatedAt),
            _ => query.OrderBy(pi => pi.PriceList.Name.ToLower()).ThenBy(pi => pi.Product.Name.ToLower())
        };
    }
}
