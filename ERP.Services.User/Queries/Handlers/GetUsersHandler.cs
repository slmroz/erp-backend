using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.User.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Queries.Handlers;
internal sealed class GetUsersHandler : IQueryHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    private readonly ErpContext _dbContext;

    public GetUsersHandler(ErpContext dbContext)
        => _dbContext = dbContext;

    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsersQuery query)
        => await _dbContext.Users
            .AsNoTracking()
            .Select(x => new UserDto(x))
            .ToListAsync();
}