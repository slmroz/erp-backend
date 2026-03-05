using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Products.Queries.Handlers;
internal sealed class GetPriceListHandler : IQueryHandler<GetPriceListQuery, PriceListDto>
{
    private readonly ErpContext _dbContext;

    public GetPriceListHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PriceListDto?> HandleAsync(GetPriceListQuery query)
    {
        return await _dbContext.PriceLists
            .AsNoTracking()
            .Include(pl => pl.PriceListItems.Where(pi => pi.RemovedAt == null))
            .Where(pl => pl.RemovedAt == null)
            .Select(pl => new PriceListDto
            {
                Id = pl.Id,
                Name = pl.Name,
                Description = pl.Description,
                CreatedAt = (DateTime)pl.CreatedAt,
                CreatedBy = pl.CreatedBy ?? 0,
                LastUpdatedAt = pl.LastUpdatedAt,
                LastUpdatedBy = pl.LastUpdatedBy,
                RemovedAt = pl.RemovedAt,
                RemovedBy = pl.RemovedBy,
                ItemCount = pl.PriceListItems.Count
            })
            .FirstOrDefaultAsync(pl => pl.Id == query.Id);
    }
}
