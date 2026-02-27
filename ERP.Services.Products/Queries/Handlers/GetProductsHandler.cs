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
        // Query 1: Count (bez Include - szybsze)
        var countQuery = _dbContext.Products.Where(p => p.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            countQuery = countQuery.Where(p =>
                p.PartNumber.ToLower().Contains(searchLower) ||
                p.Name.ToLower().Contains(searchLower) ||
                p.Oembrand.ToLower().Contains(searchLower));
        }

        if (query.GroupId.HasValue)
            countQuery = countQuery.Where(p => p.ProductGroupId == query.GroupId);

        var total = await countQuery.CountAsync();

        // Query 2: Dane z Include + paginacja
        var dataQuery = _dbContext.Products
            .Include(p => p.ProductGroup)
            .Where(p => p.RemovedAt == null);

        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            dataQuery = dataQuery.Where(p =>
                p.PartNumber.ToLower().Contains(searchLower) ||
                p.Name.ToLower().Contains(searchLower) ||
                p.Oembrand.ToLower().Contains(searchLower));
        }

        if (query.GroupId.HasValue)
            dataQuery = dataQuery.Where(p => p.ProductGroupId == query.GroupId);

        // ✅ Dynamiczne sortowanie
        dataQuery = ApplySorting(dataQuery, query.SortBy, query.SortOrder);

        var items = await dataQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                ProductGroupId = p.ProductGroupId,
                GroupName = p.ProductGroup != null ? p.ProductGroup.Name : "Unknown",
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

    private IQueryable<Product> ApplySorting(IQueryable<Product> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "partnumber" => isDescending ? query.OrderByDescending(p => p.PartNumber.ToLower()) : query.OrderBy(p => p.PartNumber.ToLower()),
            "name" => isDescending ? query.OrderByDescending(p => p.Name.ToLower()) : query.OrderBy(p => p.Name.ToLower()),
            "oembrand" => isDescending ? query.OrderByDescending(p => p.Oembrand.ToLower()) : query.OrderBy(p => p.Oembrand.ToLower()),
            "groupname" => isDescending ? query.OrderByDescending(p => p.ProductGroup.Name.ToLower()) : query.OrderBy(p => p.ProductGroup.Name.ToLower()),
            "listprice" => isDescending ? query.OrderByDescending(p => p.ListPrice) : query.OrderBy(p => p.ListPrice),
            "weightkg" => isDescending ? query.OrderByDescending(p => p.WeightKg) : query.OrderBy(p => p.WeightKg),
            "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => query.OrderBy(p => p.PartNumber.ToLower())
        };
    }
}
