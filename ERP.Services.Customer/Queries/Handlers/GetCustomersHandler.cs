using ERP.Model.Model;
using ERP.Services.Abstractions;
using ERP.Services.Abstractions.Search;
using ERP.Services.Customer.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Queries.Handlers;
public sealed class GetCustomersHandler : IQueryHandler<GetCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ErpContext _db;

    public GetCustomersHandler(ErpContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<CustomerDto>> HandleAsync(GetCustomersQuery query)
    {
        var baseQuery = _db.Customers
            .AsNoTracking()
            .Where(c => c.RemovedAt == null);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            baseQuery = baseQuery.Where(c =>
                c.Name!.Contains(query.Search) ||
                c.TaxId!.Contains(query.Search));
        }

        var total = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderBy(c => c.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                TaxId = c.TaxId,
                City = c.City,
                Country = c.Country,
                Www = c.Www,
                Facebook = c.Facebook
            })
            .ToListAsync();

        return new PagedResult<CustomerDto>
        {
            TotalCount = total,
            Items = items
        };
    }
}