using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Security;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Commands.Handlers;
internal sealed class SignUpHandler : ICommandHandler<SignUpCommand>
{
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;
    private readonly ErpContext _dbContext;

    public SignUpHandler(ErpContext dbContext, IPasswordManager passwordManager, IClock clock)
    {
        _passwordManager = passwordManager;
        _clock = clock;
        _dbContext = dbContext;
    }

    public async Task HandleAsync(SignUpCommand command)
    {
        var email = command.Email;
        var password = command.Password;

        if (await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email).ConfigureAwait(false) is not null)
        {
            throw new Exception($"User {email} already exists");
        }

        var securedPassword = _passwordManager.Secure(password);
        var user = new Model.Model.User()
        {
            CreatedAt = _clock.Current(),
            Email = email,
            Password = securedPassword,
            Role = command.Role ?? (int)Model.Enum.Role.User,
            FirstName = command.FirstName,
            LastName = command.LastName,
        };
        await _dbContext.Users.AddAsync(user).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}