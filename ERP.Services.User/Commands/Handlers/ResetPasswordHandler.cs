using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Abstractions.CQRS;
using ERP.Services.Abstractions.Security;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Commands.Handlers;
internal sealed class ResetPasswordHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly ErpContext _dbContext;
    private readonly IPasswordManager _passwordManager;
    private readonly IClock _clock;

    public ResetPasswordHandler(ErpContext dbContext, IPasswordManager passwordManager, IClock clock)
    {
        _dbContext = dbContext;
        _passwordManager = passwordManager;
        _clock = clock;
    }

    public async Task HandleAsync(ResetPasswordCommand command)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.PasswordResetToken == command.Token
                && x.PasswordResetExpires > _clock.Current())
            .ConfigureAwait(false);

        if (user is null)
        {
            throw new Exception("Invalid or expired reset token");
        }

        var newSecuredPassword = _passwordManager.Secure(command.NewPassword);
        user.Password = newSecuredPassword;
        user.PasswordResetToken = null;
        user.PasswordResetExpires = null;
        user.LastModifiedAt = _clock.Current();

        await _dbContext.SaveChangesAsync().ConfigureAwait(false);
    }
}