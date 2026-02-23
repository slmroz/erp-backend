using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Queries.Handlers;


internal sealed class GetProductGroupHandler : IQueryHandler<GetProductGroupQuery, ProductGroupDto>
{
    private readonly ErpContext _dbContext;

    public GetProductGroupHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<ProductGroupDto> HandleAsync(GetProductGroupQuery query)
    {
        return await _dbContext.ProductGroups
            .AsNoTracking()
            .Include(g => g.Products.Where(p => p.RemovedAt == null))
            .Where(g => g.RemovedAt == null)
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
            .FirstOrDefaultAsync(g => g.Id == query.Id);
    }
}
