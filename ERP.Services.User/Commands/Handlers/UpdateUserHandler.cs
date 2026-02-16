using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Commands.Handlers;
internal sealed class UpdateUserHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IClock _clock;
    private readonly ErpContext _dbContext;

    public UpdateUserHandler(ErpContext dbContext, IClock clock)
    {
        _clock = clock;
        _dbContext = dbContext;
    }

    public async Task HandleAsync(UpdateUserCommand command)
    {
        var user = await _dbContext.Users
            .Where(c => c.Id == command.Id && c.RemovedAt == null)
            .FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException("User not found");

        user.Role = command.Role;
        user.LastModifiedAt = _clock.Current();
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.Email = command.Email;

        await _dbContext.SaveChangesAsync();
    }
}