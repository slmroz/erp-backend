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

    public GetContactsHandler(ErpContext db) => _db = db;

    public async Task<PagedResult<ContactDto>> HandleAsync(GetContactsQuery query)
    {
        var search = query.Search?.Trim();
        var queryable = _db.Contacts
            .Where(c => c.RemovedAt == null);

        // Filter by CustomerId
        if (query.CustomerId.HasValue)
            queryable = queryable.Where(c => c.CustomerId == query.CustomerId);

        // ✅ Case-insensitive search
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            queryable = queryable.Where(c =>
                c.FirstName.ToLower().Contains(searchLower) ||
                c.LastName.ToLower().Contains(searchLower) ||
                c.Email.ToLower().Contains(searchLower));
        }

        var total = await queryable.CountAsync();

        // ✅ Dynamiczne sortowanie
        queryable = ApplySorting(queryable, query.SortBy, query.SortOrder);

        var items = await queryable
            .Include(c => c.Customer)
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

    private IQueryable<Contact> ApplySorting(IQueryable<Contact> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "lastname" => isDescending ? query.OrderByDescending(c => c.LastName.ToLower()) : query.OrderBy(c => c.LastName.ToLower()),
            "firstname" => isDescending ? query.OrderByDescending(c => c.FirstName.ToLower()) : query.OrderBy(c => c.FirstName.ToLower()),
            "email" => isDescending ? query.OrderByDescending(c => c.Email.ToLower()) : query.OrderBy(c => c.Email.ToLower()),
            "customer" => isDescending ? query.OrderByDescending(c => c.Customer.Name.ToLower()) : query.OrderBy(c => c.Customer.Name.ToLower()),
            "phone" => isDescending ? query.OrderByDescending(c => c.PhoneNo) : query.OrderBy(c => c.PhoneNo),
            _ => query.OrderBy(c => c.LastName.ToLower()).ThenBy(c => c.FirstName.ToLower())  // Domyślne
        };
    }
}

