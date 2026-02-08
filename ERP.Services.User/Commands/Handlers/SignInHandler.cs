using ERP.Model.Model;
using ERP.Services.Abstractions;
using ERP.Services.Abstractions.Security;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Commands.Handlers;
internal sealed class SignInHandler : ICommandHandler<SignInCommand>
{
    private readonly IAuthenticator _authenticator;
    private readonly IPasswordManager _passwordManager;
    private readonly ITokenStorage _tokenStorage;
    private readonly ErpContext _dbContext;

    public SignInHandler(ErpContext dbContext, IAuthenticator authenticator, IPasswordManager passwordManager,
        ITokenStorage tokenStorage)
    {
        _authenticator = authenticator;
        _passwordManager = passwordManager;
        _tokenStorage = tokenStorage;
        _dbContext = dbContext;
    }

    public async Task HandleAsync(SignInCommand command)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == command.Email).ConfigureAwait(false);
        if (user is null)
        {
            throw new Exception($"User {command.Email} not exists");
        }

        if (!_passwordManager.Validate(command.Password, user.Password))
        {
            throw new Exception($"Invalud credentials");
        }

        var role = (Model.Enum.Role)user.Role;
        var jwt = _authenticator.CreateToken(user.Id, role.ToString(), user.Email);
        _tokenStorage.Set(jwt);
    }
}