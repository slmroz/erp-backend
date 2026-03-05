using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;
using ERP.Services.Products.Queries;
using Microsoft.EntityFrameworkCore;

internal sealed class GetPriceListItemHandler : IQueryHandler<GetPriceListItemQuery, PriceListItemDto>
{
    private readonly ErpContext _dbContext;

    public GetPriceListItemHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PriceListItemDto> HandleAsync(GetPriceListItemQuery query)
    {
        return await _dbContext.PriceListItems
            .AsNoTracking()
            .Include(pi => pi.PriceList)
            .Include(pi => pi.Product)
            .Where(pi => pi.RemovedAt == null)
            .Select(pi => new PriceListItemDto
            {
                Id = pi.Id,
                PriceListId = pi.PriceListId,
                PriceListName = pi.PriceList.Name,
                ProductId = pi.ProductId,
                ProductName = pi.Product.Name,
                Price = (decimal)pi.Price,
                CreatedAt = (DateTime)pi.CreatedAt,
                CreatedBy = pi.CreatedBy,
                LastUpdatedAt = pi.LastUpdatedAt,
                LastUpdatedBy = pi.LastUpdatedBy,
                RemovedAt = pi.RemovedAt,
                RemovedBy = pi.RemovedBy
            })
            .FirstOrDefaultAsync(pi => pi.Id == query.Id);
    }
}
