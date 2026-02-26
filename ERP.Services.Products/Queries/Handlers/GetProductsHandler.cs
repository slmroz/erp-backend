using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Queries.Handlers;

internal sealed class GetProductsHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly ErpContext _dbContext;
    public GetProductsHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsQuery query)
    {
        // Najpierw filtrujemy BEZ Include - liczymy tylko
        var filterQuery = _dbContext.Products
            .Where(p => p.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.Search))
            filterQuery = filterQuery.Where(p =>
                p.PartNumber.ToLower().Contains(query.Search.ToLower()) ||
                p.Name.ToLower().Contains(query.Search.ToLower()) ||
                p.Oembrand.ToLower().Contains(query.Search.ToLower()));

        if (query.GroupId.HasValue)
            filterQuery = filterQuery.Where(p => p.ProductGroupId == query.GroupId);

        var total = await filterQuery.CountAsync();

        // Teraz query Z Include dla danych + paginacja
        var dataQuery = _dbContext.Products
            .Include(p => p.ProductGroup)
            .Where(p => p.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.Search))
            dataQuery = dataQuery.Where(p =>
                p.PartNumber.ToLower().Contains(query.Search.ToLower()) ||
                p.Name.ToLower().Contains(query.Search.ToLower()) ||
                p.Oembrand.ToLower().Contains(query.Search.ToLower()));

        if (query.GroupId.HasValue)
            dataQuery = dataQuery.Where(p => p.ProductGroupId == query.GroupId);

        var items = await dataQuery
            .OrderBy(p => p.PartNumber)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductGroupId = p.ProductGroupId,
                GroupName = p.ProductGroup.Name,
                PartNumber = p.PartNumber,
                Name = p.Name,
                Description = p.Description,
                OemBrand = p.Oembrand,
                ListPrice = p.ListPrice,
                WeightKg = p.WeightKg,
                CreatedAt = (DateTime)p.CreatedAt,
                LastUpdatedAt = p.LastUpdatedAt,
                RemovedAt = p.RemovedAt
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            TotalCount = total,
            Items = items
        };
    }
}
