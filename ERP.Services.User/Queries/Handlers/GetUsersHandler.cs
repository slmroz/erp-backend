using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Search;
using ERP.Services.User.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Queries.Handlers;

internal sealed class GetUsersHandler : IQueryHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly ErpContext _dbContext;

    public GetUsersHandler(ErpContext dbContext) => _dbContext = dbContext;

    public async Task<PagedResult<UserDto>> HandleAsync(GetUsersQuery query)
    {
        var baseQuery = _dbContext.Users
            .AsNoTracking()
            .Where(u => u.RemovedAt == null);

        // Case-insensitive search (Email, FirstName, LastName)
        if (!string.IsNullOrEmpty(query.Search))
        {
            var searchLower = query.Search.ToLower();
            baseQuery = baseQuery.Where(u =>
                u.Email.ToLower().Contains(searchLower) ||
                u.FirstName.ToLower().Contains(searchLower) ||
                u.LastName.ToLower().Contains(searchLower));
        }

        // Filter po roli
        if (query.Role != null)
        {
            baseQuery = baseQuery.Where(u => u.Role == query.Role);
        }

        var total = await baseQuery.CountAsync();

        // Dynamiczne sortowanie
        baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortOrder);

        var items = await baseQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new UserDto(u))
            .ToListAsync();

        return new PagedResult<UserDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<Model.Model.User> ApplySorting(IQueryable<Model.Model.User> query, string sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "lastname" => isDescending ? query.OrderByDescending(u => u.LastName.ToLower()) : query.OrderBy(u => u.LastName.ToLower()),
            "firstname" => isDescending ? query.OrderByDescending(u => u.FirstName.ToLower()) : query.OrderBy(u => u.FirstName.ToLower()),
            "email" => isDescending ? query.OrderByDescending(u => u.Email.ToLower()) : query.OrderBy(u => u.Email.ToLower()),
            "role" => isDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            _ => query.OrderBy(u => u.LastName.ToLower()).ThenBy(u => u.FirstName.ToLower())
        };
    }
}
