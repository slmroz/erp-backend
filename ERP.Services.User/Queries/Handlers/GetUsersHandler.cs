using ERP.Model.Model;
using ERP.Services.Abstractions;
using ERP.Services.User.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Queries.Handlers;
internal sealed class GetUsersHandler : IQueryHandler<GetUsers, IEnumerable<UserDto>>
{
    private readonly ErpContext _dbContext;

    public GetUsersHandler(ErpContext dbContext)
        => _dbContext = dbContext;

    public async Task<IEnumerable<UserDto>> HandleAsync(GetUsers query)
        => await _dbContext.Users
            .AsNoTracking()
            .Select(x => new UserDto(x))
            .ToListAsync();
}