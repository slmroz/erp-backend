using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
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
                c.Name!.ToLower().Contains(query.Search.ToLower()) ||
                c.TaxId!.ToLower().Contains(query.Search.ToLower()));
        }

        var total = await baseQuery.CountAsync();

        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortOrder);

        var items = await baseQuery
            //.OrderBy(c => c.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                TaxId = c.TaxId,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                Www = c.Www,
                ZipCode = c.ZipCode,
                Facebook = c.Facebook
            })
            .ToListAsync();

        return new PagedResult<CustomerDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<Model.Model.Customer> ApplySorting(IQueryable<Model.Model.Customer> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "name" => isDescending ? query.OrderByDescending(c => c.Name.ToLower()) : query.OrderBy(c => c.Name.ToLower()),
            "taxid" => isDescending ? query.OrderByDescending(c => c.TaxId.ToLower()) : query.OrderBy(c => c.TaxId.ToLower()),
            "city" => isDescending ? query.OrderByDescending(c => c.City.ToLower()) : query.OrderBy(c => c.City.ToLower()),
            "country" => isDescending ? query.OrderByDescending(c => c.Country.ToLower()) : query.OrderBy(c => c.Country.ToLower()),
            "zipcode" => isDescending ? query.OrderByDescending(c => c.ZipCode) : query.OrderBy(c => c.ZipCode),
            _ => query.OrderBy(c => c.Name.ToLower())  // Domyślne
        };
    }
}