using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Queries.Handlers;
// GetProductGroupsHandler.cs
internal sealed class GetProductGroupsHandler : IQueryHandler<GetProductGroupsQuery, PagedResult<ProductGroupDto>>
{
    private readonly ErpContext _dbContext;

    public GetProductGroupsHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<ProductGroupDto>> HandleAsync(GetProductGroupsQuery query)
    {
        // ✅ Pojedyncza query z Include - wydajniejsze
        var baseQuery = _dbContext.ProductGroups
            .Include(g => g.Products.Where(p => p.RemovedAt == null))
            .Where(g => g.RemovedAt == null);

        // Case-insensitive search
        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            baseQuery = baseQuery.Where(g => g.Name.ToLower().Contains(searchLower));
        }

        var total = await baseQuery.CountAsync();

        // ✅ Dynamiczne sortowanie
        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortOrder);

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(g => new ProductGroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreatedAt = (DateTime)g.CreatedAt,         
                LastUpdatedAt = g.LastUpdatedAt,
                RemovedAt = g.RemovedAt,
                ProductCount = g.Products.Count    // Licznik zachowany
            })
            .ToListAsync();

        return new PagedResult<ProductGroupDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<ProductGroup> ApplySorting(IQueryable<ProductGroup> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(g => g.Name.ToLower()) : query.OrderBy(g => g.Name.ToLower()),
            "productcount" => isDescending ? query.OrderByDescending(g => g.Products.Count()) : query.OrderBy(g => g.Products.Count()),
            "createdat" => isDescending ? query.OrderByDescending(g => g.CreatedAt) : query.OrderBy(g => g.CreatedAt),
            "lastupdatedat" => isDescending ? query.OrderByDescending(g => g.LastUpdatedAt) : query.OrderBy(g => g.LastUpdatedAt),
            _ => query.OrderBy(g => g.Name.ToLower())
        };
    }
}
