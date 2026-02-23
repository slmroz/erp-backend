using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Products.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Products.Queries.Handlers;
internal sealed class GetProductHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    private readonly ErpContext _dbContext;

    public GetProductHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<ProductDto> HandleAsync(GetProductQuery query)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Include(p => p.ProductGroup)
            .Where(p => p.RemovedAt == null)
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
            .FirstOrDefaultAsync(p => p.Id == query.Id);

        return product;
    }
}

