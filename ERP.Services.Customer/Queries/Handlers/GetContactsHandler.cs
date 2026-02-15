using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.Customer.Commands;
using ERP.Services.Customer.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Customer.Queries.Handlers;
internal sealed class GetContactsHandler : IQueryHandler<GetContactsQuery, PagedResult<ContactDto>>
{
    private readonly ErpContext _db;

    public GetContactsHandler(ErpContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ContactDto>> HandleAsync(GetContactsQuery query)
    {
        var search = query.Search?.Trim();
        var queryable = _db.Contacts
            .Where(c => c.RemovedAt == null);

        // Filter by CustomerId if provided
        if (query.CustomerId.HasValue)
            queryable = queryable.Where(c => c.CustomerId == query.CustomerId);

        // Search in FirstName, LastName, Email
        if (!string.IsNullOrEmpty(search))
        {   
            queryable = queryable.Where(c =>
                c.FirstName.Contains(search) ||
                c.LastName.Contains(search) ||
                c.Email.Contains(search));
        }

        queryable = queryable.Include(c => c.Customer);

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new ContactDto(
                c.Id,
                c.CustomerId,
                c.FirstName!,
                c.LastName!,
                c.PhoneNo,
                c.Email,
                c.Customer != null ? c.Customer.Name : null
            ))
            .ToListAsync();

        return new PagedResult<ContactDto>
        {
            TotalCount = total,
            Items = items
        };
    }
}
