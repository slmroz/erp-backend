using ERP.Model.Model;
using ERP.Services.Abstractions;
using ERP.Services.User.DTO;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Queries.Handlers;
internal sealed class GetUserHandler : IQueryHandler<GetUserQuery, UserDto>
{
    private readonly ErpContext _dbContext;

    public GetUserHandler(ErpContext dbContext)
        => _dbContext = dbContext;

    public async Task<UserDto> HandleAsync(GetUserQuery query)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == query.UserId);

        return new UserDto(user);
    }
}