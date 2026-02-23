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
        var filterQuery = _dbContext.ProductGroups.Where(g => g.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.Search))
            filterQuery = filterQuery.Where(g => g.Name.Contains(query.Search));

        var total = await filterQuery.CountAsync();

        var dataQuery = _dbContext.ProductGroups
            .Include(g => g.Products.Where(p => p.RemovedAt == null))
            .Where(g => g.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.Search))
            dataQuery = dataQuery.Where(g => g.Name.Contains(query.Search));

        var items = await dataQuery
            .OrderBy(g => g.Name)
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
                ProductCount = g.Products.Count
            })
            .ToListAsync();

        return new PagedResult<ProductGroupDto>
        {
            TotalCount = total,
            Items = items
        };

    }
}
