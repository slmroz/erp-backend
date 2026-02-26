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
// GetCurrencyHandler.cs
internal sealed class GetCurrencyHandler : IQueryHandler<GetCurrencyQuery, CurrencyDto>
{
    private readonly ErpContext _dbContext;

    public GetCurrencyHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<CurrencyDto> HandleAsync(GetCurrencyQuery query)
    {
        return await _dbContext.Currencies
            .AsNoTracking()
            .Where(c => c.RemovedAt == null)
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
            .FirstOrDefaultAsync(c => c.Id == query.Id);
    }
}

